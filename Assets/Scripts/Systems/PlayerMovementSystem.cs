using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

public class PlayerMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.
            WithAll<PlayerTag>().
            ForEach((ref PhysicsVelocity physics, ref PhysicsMass mass, ref Translation pos,ref Rotation rotation, in MoveData moveData) =>
            {
                if(moveData.direction.x == 0 && moveData.direction.z == 0)
                {
                    physics.Linear.x = 0;
                    physics.Linear.z = 0;
                }
                else
                {
                    float2 curInput = new float2(moveData.direction.x, moveData.direction.z);
                    float2 newVel = physics.Linear.xz;

                    newVel += curInput * moveData.speed * deltaTime;

                    physics.Linear.xz = newVel;


                    if(physics.Linear.x > moveData.maxVelocity)
                    {
                        physics.Linear.x = moveData.maxVelocity;
                    }
                    if(physics.Linear.x < -moveData.maxVelocity)
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
