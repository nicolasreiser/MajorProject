using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class RotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entity unit = Entity.Null;
        Translation playerPosition = new Translation();
        bool playerIsMoving = false;

        Entities.
            WithAny<PlayerTag>().
            ForEach((ref Entity entity, ref Translation position, ref Rotation rotation, in MoveData moveData) =>
            {
                unit = entity;
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
            WithAny<PlayerTag, EnemyTag>().
            ForEach((ref Entity entity, ref Translation position, ref Rotation rotation, in MoveData moveData) =>
            {
                if (!moveData.direction.Equals(float3.zero))
                {
                    quaternion targetRotation = quaternion.LookRotationSafe(moveData.direction, math.up());

                    //targetRotation.value.z = 0;
                    //targetRotation.value.x = 0;

                    rotation.Value = math.slerp(rotation.Value, targetRotation, moveData.turnSpeed * deltaTime);
                }

            }).Run();

        float3 unitPosition = playerPosition.Value;
        Entity closestEnemy = Entity.Null;
        float3 closestTargetPosition = float3.zero;

        // get the closest enemy 
        Entities.
        WithAll<EnemyTag>().
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
            ForEach((ref Entity entity, ref Translation position, ref Rotation rotation, in MoveData moveData) =>
            {
                
                    // set target rotation
                    float3 dirToTarget = playerPosition.Value - position.Value;

                    quaternion targetRotation = quaternion.LookRotationSafe(dirToTarget, math.up());

                    // remove unwanted rotations
                    targetRotation.value.z = 0;
                    targetRotation.value.x = 0;

                    rotation.Value = math.slerp(rotation.Value, targetRotation, moveData.turnSpeed * deltaTime);

                    
            }).Run();
    }
}
