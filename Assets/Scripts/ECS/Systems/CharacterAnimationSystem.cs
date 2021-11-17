using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


public class CharacterAnimationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities
           .WithoutBurst()
           .ForEach((
               ref Entity entity,
               ref SimpleAnimation simpleAnimation,
               ref DynamicBuffer<SimpleAnimationClipData> simpleAnimationClipDatas,
               ref Translation transform,
               ref Rotation rotation,
               in AnimationStateComponent anim) =>
           {

               if (anim.AnimationState == AnimState.Run)
               {
                   quaternion targetRotation = quaternion.LookRotationSafe(new float3(-0.65f,0,0.35f), math.up());

                   rotation.Value = math.slerp(rotation.Value, targetRotation, anim.TurnSpeed * deltaTime);

                   Debug.Log("Rotation of : " + targetRotation + "on entity : " + entity.Index);
                   simpleAnimation.TransitionTo(1, .2f, ref simpleAnimationClipDatas, false);
                   simpleAnimation.SetSpeed(1f, 1, ref simpleAnimationClipDatas);
               }
               else
               {
                   quaternion targetRotation = quaternion.LookRotationSafe(new float3(0, 0, 0), math.up());

                   rotation.Value = math.slerp(rotation.Value, targetRotation, anim.TurnSpeed * deltaTime);

                   simpleAnimation.TransitionTo(0, .2f, ref simpleAnimationClipDatas, false);
                   simpleAnimation.SetSpeed(1f, 0, ref simpleAnimationClipDatas);
               }


           }).Run();
    }

}
