using Unity.Animation.Hybrid;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Animation;

#if UNITY_EDITOR
[Serializable]
public class SimpleAnimationClipAuthoring
{
    public AnimationClip Clip;
    public bool Loop = false;
    public float InitialSpeed = 1f;
    public bool ComputeRootmotionDeltas = false;
}

[DisallowMultipleComponent]
[UpdateInGroup(typeof(GameObjectConversionGroup))]
[UpdateAfter(typeof(RigConversion))]
public class SimpleAnimationAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject RootMotionBone;
    public List<SimpleAnimationClipAuthoring> Clips = new List<SimpleAnimationClipAuthoring>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        SimpleAnimationAuthoring.SetupSimpleAnimationOnEntity(entity, dstManager, conversionSystem, gameObject, Clips, RootMotionBone);
    }

    public static void SetupSimpleAnimationOnEntity(
        Entity entity,
        EntityManager dstManager,
        GameObjectConversionSystem conversionSystem,
        GameObject gameObject,
        List<SimpleAnimationClipAuthoring> Clips,
        GameObject rootMotionBone)
    {
        DynamicBuffer<SimpleAnimationClipData> inputsBuffer = dstManager.AddBuffer<SimpleAnimationClipData>(entity);
        for (int i = 0; i < Clips.Count; i++)
        {
            conversionSystem.DeclareAssetDependency(gameObject, Clips[i].Clip);

            SimpleAnimationClipData a = new SimpleAnimationClipData
            {
                Clip = Clips[i].Clip.ToDenseClip(),
                Speed = Clips[i].InitialSpeed,
                Weight = (i == 0) ? 1f : 0f,
                Loop = Clips[i].Loop,
                Time = 0f,
                HasRootMotion = Clips[i].ComputeRootmotionDeltas,
            };

            inputsBuffer.Add(a);
        }

        SimpleAnimation simpleAnimaition = default;

        if (rootMotionBone)
        {
            RigComponent rigComponent = gameObject.GetComponent<RigComponent>();
            for (var boneIter = 0; boneIter < rigComponent.Bones.Length; boneIter++)
            {
                if (rootMotionBone.name == rigComponent.Bones[boneIter].name)
                {
                    simpleAnimaition.RootMotionBone = RigGenerator.ComputeRelativePath(rigComponent.Bones[boneIter], rigComponent.transform);
                }
            }

            if (simpleAnimaition.RootMotionBone == default)
            {
                UnityEngine.Debug.LogError("Root motion bone could not be found");
            }

            dstManager.AddComponent<ProcessDefaultAnimationGraph.AnimatedRootMotion>(entity);
        }

        dstManager.AddComponent<SimpleAnimationDeltaTime>(entity);
        dstManager.AddComponentData(entity, simpleAnimaition);
        //dstManager.AddComponent<DisableRootTransformReadWriteTag>(entity);
    }
}
#endif