using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[Serializable]
[GenerateAuthoringComponent]
public struct SampleAnimationController : IComponentData
{
    public float TransitionDelay;
    public float TransitionDuration;

    [HideInInspector]
    public float _lastTimeSwitchedAnimation;
    [HideInInspector]
    public int _currentClipIndex;
}

public class SampleAnimationControlerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float time = (float)Time.ElapsedTime;

        Entities
            .ForEach((
                ref SimpleAnimation simpleAnimation,
                ref DynamicBuffer<SimpleAnimationClipData> simpleAnimationClipDatas,
                ref SampleAnimationController animationController) =>
            {
                if (time >= animationController._lastTimeSwitchedAnimation + animationController.TransitionDelay)
                {
                    animationController._currentClipIndex = (animationController._currentClipIndex == 0) ? 1 : 0;
                    simpleAnimation.TransitionTo(animationController._currentClipIndex, animationController.TransitionDuration, ref simpleAnimationClipDatas, false);
                    simpleAnimation.SetSpeed(1f, animationController._currentClipIndex, ref simpleAnimationClipDatas);

                    animationController._lastTimeSwitchedAnimation = time;
                }
            }).Schedule();
    }
}
