using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class MoveForwardSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.WithAll<MoveForwardData>()
            .ForEach((ref Translation trans, ref Rotation rot, ref MoveForwardData moveForward) =>
            {
                trans.Value += moveForward.velocity * deltaTime * math.forward(rot.Value);
            }).ScheduleParallel();
    }
}
