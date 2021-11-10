using Unity.Animation;
using Unity.Burst;
using Unity.DataFlowGraph;
using Unity.Entities;
using Unity.Mathematics;

public struct SimpleAnimation : IComponentData
{
    public StringHash RootMotionBone;

    public bool IsTransitioning;
    public float RemainingTransitionTime;
    public float TotalTransitionTime;
    public int FromClipIndex;
    public int ToClipIndex;

    public bool TransitionTo(int toClip, float duration, ref DynamicBuffer<SimpleAnimationClipData> clipDatas, bool resetTime, bool force = false)
    {
        if (force || ToClipIndex != toClip)
        {
            bool isInstant = duration <= 0f;

            SetWeight(0f, FromClipIndex, ref clipDatas);
            SetWeight(0f, ToClipIndex, ref clipDatas);

            if (!isInstant)
            {
                IsTransitioning = true;
                RemainingTransitionTime = duration;
                TotalTransitionTime = duration;
            }

            FromClipIndex = ToClipIndex;
            ToClipIndex = toClip;

            if (isInstant)
            {
                SetWeight(0f, FromClipIndex, ref clipDatas);
                SetWeight(1f, ToClipIndex, ref clipDatas);
            }
            else
            {
                SetWeight(1f, FromClipIndex, ref clipDatas);
                SetWeight(0f, ToClipIndex, ref clipDatas);
            }

            if (resetTime)
            {
                SetTime(0f, ToClipIndex, ref clipDatas);
            }

            return true;
        }

        return false;
    }

    public void SetSpeed(float value, int clipIndex, ref DynamicBuffer<SimpleAnimationClipData> clipDatas)
    {
        var i = clipDatas[clipIndex];
        if (value != i.Speed)
        {
            i.Speed = value;
            i.SpeedDirty = true;
            clipDatas[clipIndex] = i;
        }
    }

    public float GetSpeed(int clipIndex, ref DynamicBuffer<SimpleAnimationClipData> clipDatas)
    {
        return clipDatas[clipIndex].Speed;
    }

    public void SetWeight(float value, int clipIndex, ref DynamicBuffer<SimpleAnimationClipData> clipDatas)
    {
        var i = clipDatas[clipIndex];
        if (value != i.Weight)
        {
            i.Weight = value;
            i.WeightDirty = true;
            clipDatas[clipIndex] = i;
        }
    }

    public float GetWeight(int clipIndex, ref DynamicBuffer<SimpleAnimationClipData> clipDatas)
    {
        return clipDatas[clipIndex].Weight;
    }

    public void SetTime(float value, int clipIndex, ref DynamicBuffer<SimpleAnimationClipData> clipDatas)
    {
        var i = clipDatas[clipIndex];
        if (value != i.Time)
        {
            i.Time = value;
            i.TimeDirty = true;
            clipDatas[clipIndex] = i;
        }
    }

    public float GetTime(int clipIndex, ref DynamicBuffer<SimpleAnimationClipData> clipDatas)
    {
        return clipDatas[clipIndex].Time;
    }

    public float GetNormalizedTime(int clipIndex, ref DynamicBuffer<SimpleAnimationClipData> clipDatas)
    {
        float clipDuration = GetClipDuration(clipIndex, ref clipDatas);
        float time = GetTime(clipIndex, ref clipDatas);
        if (clipDatas[clipIndex].Loop)
        {
            return (time - (clipDuration * math.floor(time / clipDuration))) / clipDuration;
        }
        else
        {
            return time / clipDuration;
        }
    }

    public float GetClipDuration(int clipIndex, ref DynamicBuffer<SimpleAnimationClipData> clipDatas)
    {
        return clipDatas[clipIndex].Clip.Value.Duration;
    }
}

public struct SimpleAnimationClipData : IBufferElementData
{
    public BlobAssetReference<Clip> Clip;
    public NodeHandle<ClipPlayerNode> ClipNode;

    public bool HasRootMotion;
    public bool Loop;

    public float Speed;
    public bool SpeedDirty;
    public float Weight;
    public bool WeightDirty;
    public float Time;
    public bool TimeDirty;
}

public struct SimpleAnimationGraphData : ISystemStateComponentData
{
    public NodeHandle<ComponentNode> EntityNode;
    public NodeHandle<ConvertSimpleAnimationDeltaTimeToFloatNode> DeltaTimeNode;
    public NodeHandle<NMixerNode> NMixerNode;
}

public struct SimpleAnimationDeltaTime : IComponentData
{
    public float Value;
}

public class ConvertSimpleAnimationDeltaTimeToFloatNode : ConvertToBase<
    ConvertSimpleAnimationDeltaTimeToFloatNode,
    SimpleAnimationDeltaTime,
    float,
    ConvertSimpleAnimationDeltaTimeToFloatNode.Kernel>
{
    [BurstCompile]
    public struct Kernel : IGraphKernel<KernelData, KernelDefs>
    {
        public void Execute(RenderContext ctx, in KernelData data, ref KernelDefs ports)
        {
            ctx.Resolve(ref ports.Output) = ctx.Resolve(ports.Input).Value;
        }
    }
}