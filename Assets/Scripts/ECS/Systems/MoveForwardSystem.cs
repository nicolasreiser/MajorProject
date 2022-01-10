using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

public class MoveForwardSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.
            WithAll<MoveForwardData>().
            WithNone<PausedTag>().
            ForEach((ref PhysicsVelocity physics ,ref Translation trans, ref Rotation rot, ref MoveForwardData moveForward) =>
            {
                trans.Value += moveForward.velocity * deltaTime * math.forward(rot.Value);

                //physics.Angular = float3.zero;
                //physics.Linear += deltaTime * moveForward.velocity * math.forward(rot.Value);

            }).ScheduleParallel();
    }
}
