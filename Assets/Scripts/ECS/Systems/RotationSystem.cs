using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// system rotating entities (players and enemies)
public class RotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Translation playerPosition = new Translation();
        bool playerIsMoving = false;

        Entities.
            WithAny<PlayerTag>().
            WithNone<PausedTag>().
            ForEach((ref Entity entity, ref Translation position, ref Rotation rotation, in MoveData moveData) =>
            {
                playerPosition = position;
                if (!moveData.direction.Equals(float3.zero))
                {
                    playerIsMoving = true;
                }
                else
                {
                    playerIsMoving = false;
                }
            }).Run();
               
        // rotate body towards move direction


        Entities.
            WithAny<PlayerTag,EnemyTag>().
            WithNone<AttackState, PausedTag>().
            ForEach((ref Entity entity, ref Translation position, ref Rotation rotation, in MoveData moveData) =>
            {
                if (!moveData.direction.Equals(float3.zero))
                {
                    quaternion targetRotation = quaternion.LookRotationSafe(moveData.direction, math.up());

                    
                    rotation.Value = math.slerp(rotation.Value, targetRotation, moveData.turnSpeed * deltaTime);
                
                }

            }).Run();

        float3 unitPosition = playerPosition.Value;
        Entity closestEnemy = Entity.Null;
        float3 closestTargetPosition = float3.zero;


        // get the closest enemy 

        Entities.
        WithAll<EnemyTag>().
        WithNone<PausedTag>().
        ForEach((ref Entity targetEntity, ref Translation enemyPosition) =>
        {
            if (closestEnemy == Entity.Null)
            {
                // no target
                closestEnemy = targetEntity;
                closestTargetPosition = enemyPosition.Value;
            }
            else if (math.distance(unitPosition, enemyPosition.Value) < math.distance(unitPosition, closestTargetPosition))
            {
                // closest target
                closestEnemy = targetEntity;
                closestTargetPosition = enemyPosition.Value;
            }
        }).Run();

        // rotate towards closest enemy if not moving
        Entities.
            WithAll<PlayerTag>().
            WithNone<PausedTag>().
            ForEach((ref Entity entity, ref Translation position, ref Rotation rotation, in MoveData moveData) =>
            {
                if (closestEnemy != Entity.Null && !playerIsMoving)
                {
                    // set target rotation
                    float3 dirToTarget = closestTargetPosition - position.Value;
                    quaternion targetRotation = quaternion.LookRotationSafe(dirToTarget, math.up());

                    // remove unwanted rotations
                    targetRotation.value.z = 0;
                    targetRotation.value.x = 0;

                    rotation.Value = math.slerp(rotation.Value, targetRotation, moveData.turnSpeed * deltaTime);

                    // helper lines
                    Debug.DrawLine(position.Value, closestTargetPosition);
                }
            }).Run();


        // rotate attacking enemies towards player

        Entities.WithAll<EnemyTag, AttackState>().
            WithNone<PausedTag>().
            ForEach((ref Entity entity, ref Translation position, ref Rotation rotation, in MoveData moveData) =>
            {
                
                    // set target rotation
                    float3 dirToTarget = playerPosition.Value - position.Value;

                    quaternion targetRotation = quaternion.LookRotationSafe(dirToTarget, math.up());

                targetRotation.value.z = 0;
                targetRotation.value.x = 0;

                rotation.Value = math.slerp(rotation.Value, targetRotation, moveData.turnSpeed * deltaTime);

                    // remove unwanted rotations
                    targetRotation.value.z = 0;
                    targetRotation.value.y = 0;
                    targetRotation.value.x = 0;


                Debug.DrawLine(position.Value, playerPosition.Value);


            }).Run();
    }
}
