using Unity.Animation;
using Unity.Animation.Hybrid;
using Unity.Burst;
using Unity.Collections;
using Unity.DataFlowGraph;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(DefaultAnimationSystemGroup))]
public class SimpleAnimationSystem : SystemBase
{
    private ProcessDefaultAnimationGraph _processDefaultAnimationGraphSystem;
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _processDefaultAnimationGraphSystem = World.GetOrCreateSystem<ProcessDefaultAnimationGraph>();
        _processDefaultAnimationGraphSystem.AddRef();
        _processDefaultAnimationGraphSystem.Set.RendererModel = NodeSet.RenderExecutionModel.Islands;

        _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnDestroy()
    {
        if (_processDefaultAnimationGraphSystem == null)
            return;

        EntityQuery graphsQuery = GetEntityQuery(typeof(SimpleAnimationGraphData), typeof(SimpleAnimationClipData));
        NativeArray<Entity> graphEntities = graphsQuery.ToEntityArray(Allocator.Temp);
        for (int i = 0; i < graphEntities.Length; i++)
        {
            Entity graphEntity = graphEntities[i];
            SimpleAnimationGraphData simpleAnimationGraphData = EntityManager.GetComponentData<SimpleAnimationGraphData>(graphEntity);
            DynamicBuffer<SimpleAnimationClipData> clipDatas = EntityManager.GetBuffer<SimpleAnimationClipData>(graphEntity);
            DestroyGraph(_processDefaultAnimationGraphSystem, ref simpleAnimationGraphData, ref clipDatas);
        }
        graphEntities.Dispose();

        _processDefaultAnimationGraphSystem.RemoveRef();

        base.OnDestroy();
    }

    protected override void OnUpdate()
    {
        CompleteDependency();

        float deltaTime = Time.DeltaTime;
        EntityCommandBuffer ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        Entities
            .WithName("CreateGraph")
            .WithNone<SimpleAnimationGraphData>()
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity e, ref Rig rig, ref SimpleAnimation simpleAnimation) =>
            {
                SimpleAnimationGraphData graphData = CreateGraph(EntityManager, e, _processDefaultAnimationGraphSystem, in simpleAnimation, ref rig);
                ecb.AddComponent(e, graphData);
            }).Run();

        Entities
            .WithName("DestroyGraph")
            .WithNone<SimpleAnimation>()
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity e, ref SimpleAnimationGraphData graphData, ref DynamicBuffer<SimpleAnimationClipData> clipDatas) =>
            {
                DestroyGraph(_processDefaultAnimationGraphSystem, ref graphData, ref clipDatas);
                ecb.RemoveComponent<SimpleAnimationGraphData>(e);
            }).Run();

        Dependency = Entities
            .ForEach((
                Entity entity,
                ref SimpleAnimation simpleAnimation,
                ref SimpleAnimationDeltaTime dt,
                ref DynamicBuffer<SimpleAnimationClipData> simpleAnimationClipDatas) =>
            {
                // Update time
                dt.Value = deltaTime;

                // Handle transitions
                if (simpleAnimation.IsTransitioning)
                {
                    simpleAnimation.RemainingTransitionTime -= deltaTime;
                    float normalizedTransitionTime = math.clamp(1f - (simpleAnimation.RemainingTransitionTime / simpleAnimation.TotalTransitionTime), 0f, 1f);

                    simpleAnimation.SetWeight(1f - normalizedTransitionTime, simpleAnimation.FromClipIndex, ref simpleAnimationClipDatas);
                    simpleAnimation.SetWeight(normalizedTransitionTime, simpleAnimation.ToClipIndex, ref simpleAnimationClipDatas);

                    if (simpleAnimation.RemainingTransitionTime <= 0f)
                    {
                        simpleAnimation.IsTransitioning = false;
                    }
                }
            }).Schedule(Dependency);
        Dependency.Complete();

        // Update graph values
        NodeSet set = _processDefaultAnimationGraphSystem.Set;
        Entities
            .WithoutBurst()
            .ForEach((
                Entity entity,
                ref SimpleAnimationGraphData simpleAnimationGraphData,
                ref DynamicBuffer<SimpleAnimationClipData> simpleAnimationClipDatas) =>
            {
                // Handle setting and outputting values
                for (int i = 0; i < simpleAnimationClipDatas.Length; ++i)
                {
                    SimpleAnimationClipData clipData = simpleAnimationClipDatas[i];

                    // Set weights
                    if (clipData.WeightDirty)
                    {
                        set.SetData(simpleAnimationGraphData.NMixerNode, NMixerNode.KernelPorts.Weights, i, clipData.Weight);
                        clipData.WeightDirty = false;
                    }

                    // Set speeds
                    if (clipData.SpeedDirty)
                    {
                        set.SetData(clipData.ClipNode, ClipPlayerNode.KernelPorts.Speed, clipData.Speed);
                        clipData.SpeedDirty = false;
                    }

                    // Set Time
                    if (clipData.TimeDirty)
                    {
                        set.SendMessage(clipData.ClipNode, ClipPlayerNode.SimulationPorts.Time, clipData.Time);
                        clipData.TimeDirty = false;
                    }

                    // TODO: Get time
                    clipData.Time += deltaTime * clipData.Speed;

                    simpleAnimationClipDatas[i] = clipData;
                }
            }).Run();
    }

    private static SimpleAnimationGraphData CreateGraph(
        EntityManager entityManager,
        Entity entity,
        ProcessDefaultAnimationGraph graphSystem,
        in SimpleAnimation simpleAnimation,
        ref Rig rig)
    {
        GraphHandle graphHandle = graphSystem.CreateGraph();
        NodeSet set = graphSystem.Set;

        SimpleAnimationGraphData graphData = new SimpleAnimationGraphData
        {
            EntityNode = set.CreateComponentNode(entity),
            DeltaTimeNode = set.Create<ConvertSimpleAnimationDeltaTimeToFloatNode>(),
            NMixerNode = set.Create<NMixerNode>(),
        };

        DynamicBuffer<SimpleAnimationClipData> animationClipDatas = entityManager.GetBuffer<SimpleAnimationClipData>(entity);

        // Time
        set.Connect(graphData.EntityNode, graphData.DeltaTimeNode, ConvertSimpleAnimationDeltaTimeToFloatNode.KernelPorts.Input, NodeSet.ConnectionType.Feedback);

        // NMixer
        {
            // Rig
            set.SendMessage(graphData.NMixerNode, NMixerNode.SimulationPorts.Rig, rig);

            // Inputs & Weights size
            set.SetPortArraySize(graphData.NMixerNode, NMixerNode.KernelPorts.Inputs, animationClipDatas.Length);
            set.SetPortArraySize(graphData.NMixerNode, NMixerNode.KernelPorts.Weights, animationClipDatas.Length);

            // Set weights
            for (int i = 0; i < animationClipDatas.Length; ++i)
            {
                set.SetData(graphData.NMixerNode, NMixerNode.KernelPorts.Weights, i, animationClipDatas[i].Weight);
            }

            // Connect back to entity node
            set.Connect(graphData.NMixerNode, NMixerNode.KernelPorts.Output, graphData.EntityNode, NodeSetAPI.ConnectionType.Feedback);
        }

        // Setup clip nodes
        for (int i = 0; i < animationClipDatas.Length; i++)
        {
            SimpleAnimationClipData clipData = animationClipDatas[i];

            clipData.ClipNode = set.Create<ClipPlayerNode>();
            set.Connect(graphData.DeltaTimeNode, ConvertSimpleAnimationDeltaTimeToFloatNode.KernelPorts.Output, clipData.ClipNode, ClipPlayerNode.KernelPorts.DeltaTime);
            set.Connect(clipData.ClipNode, ClipPlayerNode.KernelPorts.Output, graphData.NMixerNode, NMixerNode.KernelPorts.Inputs, i);

            set.SendMessage(clipData.ClipNode, ClipPlayerNode.SimulationPorts.Configuration, GetClipConfiguration(in clipData, simpleAnimation.RootMotionBone));
            set.SendMessage(clipData.ClipNode, ClipPlayerNode.SimulationPorts.Rig, rig);
            set.SendMessage(clipData.ClipNode, ClipPlayerNode.SimulationPorts.Clip, clipData.Clip);
            set.SetData(clipData.ClipNode, ClipPlayerNode.KernelPorts.Speed, clipData.Speed);

            animationClipDatas[i] = clipData;
        }

        return graphData;
    }

    private static ClipConfiguration GetClipConfiguration(in SimpleAnimationClipData clipData, StringHash rootMotionBone)
    {
        ClipConfiguration clipConfig = new ClipConfiguration();
        if (clipData.HasRootMotion)
        {
            clipConfig.Mask |= ClipConfigurationMask.DeltaRootMotion;
        }

        if (clipData.Loop)
        {
            clipConfig.Mask |= ClipConfigurationMask.LoopTime;
        }

        clipConfig.MotionID = rootMotionBone;

        return clipConfig;
    }

    private static void DestroyGraph(ProcessDefaultAnimationGraph graphSystem, ref SimpleAnimationGraphData graphData, ref DynamicBuffer<SimpleAnimationClipData> animationClipDataBuffer)
    {
        NodeSet set = graphSystem.Set;

        for (int i = 0; i < animationClipDataBuffer.Length; i++)
        {
            set.Destroy(animationClipDataBuffer[i].ClipNode);
        }

        set.Destroy(graphData.DeltaTimeNode);
        set.Destroy(graphData.EntityNode);
        set.Destroy(graphData.NMixerNode);
    }

    public static void NormalizeWeights(in SimpleAnimation simpleAnimation, ref DynamicBuffer<SimpleAnimationClipData> animationClipDataBuffer)
    {
        float totalWeight = 0f;
        for (int i = 0; i < animationClipDataBuffer.Length; i++)
        {
            totalWeight += animationClipDataBuffer[i].Weight;
        }

        if (totalWeight <= 0f)
        {
            simpleAnimation.SetWeight(1f, 0, ref animationClipDataBuffer);
        }
        else
        {
            for (int i = 0; i < animationClipDataBuffer.Length; i++)
            {
                float weight = simpleAnimation.GetWeight(i, ref animationClipDataBuffer);
                if (weight > 0f)
                {
                    simpleAnimation.SetWeight(weight / totalWeight, i, ref animationClipDataBuffer);
                }
            }
        }
    }
}
