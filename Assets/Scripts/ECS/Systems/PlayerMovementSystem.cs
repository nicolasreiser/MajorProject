using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

public class PlayerMovementSystem : SystemBase
{
    private bool StartedMoving = false;
    private bool hasCastMove = false;
    private bool hasCastIdle = false;
    protected override void OnCreate()
    {
        base.OnCreate();
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.
            WithoutBurst().
            WithAll<PlayerTag>().
            ForEach((ref PhysicsVelocity physics, ref PhysicsMass mass, ref Translation pos, ref Rotation rotation, in MoveData moveData) =>
            {
                if (moveData.direction.x == 0 && moveData.direction.z == 0)
                {
                    StartedMoving = false;

                    physics.Linear.x = 0;
                    physics.Linear.z = 0;
                }
                else
                {
                    StartedMoving = true;

                    float2 curInput = new float2(moveData.direction.x, moveData.direction.z);
                    float2 newVel = physics.Linear.xz;

                    newVel += curInput * moveData.speed * deltaTime;

                    physics.Linear.xz = newVel;


                    if (physics.Linear.x > moveData.maxVelocity)
                    {
                        physics.Linear.x = moveData.maxVelocity;
                    }
                    if (physics.Linear.x < -moveData.maxVelocity)
                    {
                        physics.Linear.x = -moveData.maxVelocity;
                    }
                    if (physics.Linear.z > moveData.maxVelocity)
                    {
                        physics.Linear.z = moveData.maxVelocity;
                    }
                    if (physics.Linear.z < -moveData.maxVelocity)
                    {
                        physics.Linear.z = -moveData.maxVelocity;
                    }
                }

                mass.InverseInertia[0] = 0;
                mass.InverseInertia[2] = 0;



            }).Run();

        // animation part

        Entities
            .WithoutBurst()
            .WithAll<PlayerTag>()
            .ForEach((
                ref SimpleAnimation simpleAnimation,
                ref DynamicBuffer<SimpleAnimationClipData> simpleAnimationClipDatas,
                in MoveData moveData) =>
            {
                if(!StartedMoving && !hasCastMove)
                {
                    simpleAnimation.TransitionTo(1, .2f, ref simpleAnimationClipDatas, false);
                    simpleAnimation.SetSpeed(1f, 1, ref simpleAnimationClipDatas);
                    hasCastMove = true;
                    hasCastIdle = false;
                }
                else if(!hasCastIdle)
                {
                    simpleAnimation.TransitionTo(0, .2f, ref simpleAnimationClipDatas, false);
                    simpleAnimation.SetSpeed(1f, 0, ref simpleAnimationClipDatas);
                    hasCastMove = false;
                    hasCastIdle = true;
                }


            }).Run();
    }
}
