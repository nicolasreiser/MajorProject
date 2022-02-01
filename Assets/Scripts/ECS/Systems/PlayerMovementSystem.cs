using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

public class PlayerMovementSystem : SystemBase
{
    
    EndSimulationEntityCommandBufferSystem esecb;
    protected override void OnCreate()
    {
        base.OnCreate();
         esecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        var ecb = esecb.CreateCommandBuffer();
        Entities.
            WithoutBurst().
            WithAll<PlayerTag>().
            WithNone<PausedTag>().
            ForEach((ref PhysicsVelocity physics, ref PhysicsMass mass, ref Translation pos, ref Rotation rotation, ref AnimationHolderComponent anim ,in MoveData moveData) =>
            {
                if (moveData.direction.x == 0 && moveData.direction.z == 0)
                {
                    var animation = anim.entity;

                    AnimationStateComponent animState = new AnimationStateComponent();

                    animState.AnimationState = AnimState.Shoot;
                    animState.TurnSpeed = anim.TurnSpeed;

                    ecb.SetComponent(animation, animState);


                    physics.Linear.x = 0;
                    physics.Linear.z = 0;
                }
                else
                {
                    var animation = anim.entity;

                    AnimationStateComponent animState = new AnimationStateComponent();

                    animState.AnimationState = AnimState.Run;
                    animState.TurnSpeed = anim.TurnSpeed;

                    ecb.SetComponent(animation, animState);


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

        
    }
}
