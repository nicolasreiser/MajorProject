using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Collections;
using Unity.Jobs;

public class PlayerAttackSystem : SystemBase
{
        EntityQuery playerQuery;
    
    protected override void OnUpdate()
    {
        Entity projectile = Entity.Null;
        float deltaTime = Time.DeltaTime;

        Translation playerPosition = new Translation();

        Entities.ForEach((PrefabEntityStorage prefabs) =>
        {
            projectile = prefabs.PlayerProjectile;
        }).Run();

        // player poisition

        Entities.
            WithAny<PlayerTag>().
            ForEach((ref Entity entity, ref Translation position, ref Rotation rotation, in MoveData moveData) =>
            {
                playerPosition = position;
            }).Run();

        // player query


        playerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<PlayerTag>() }
        });

        // get range to closest enemy
        Entity closestEnemy = Entity.Null;
        float3 closestTargetPosition = float3.zero;
        float closestEnemyDistance = 0;


        Entities.
        WithAll<EnemyTag>().
        ForEach(( Entity targetEntity, ref Translation enemyPosition) =>
        {
            if (closestEnemy == Entity.Null)
            {;
                // no target
                closestEnemy = targetEntity;
                closestTargetPosition = enemyPosition.Value;
                closestEnemyDistance = math.distance(playerPosition.Value, enemyPosition.Value);

            }
            else if (math.distance(playerPosition.Value, enemyPosition.Value) < math.distance(playerPosition.Value, closestTargetPosition))
            {
                // closest target
                closestEnemy = targetEntity;
                closestTargetPosition = enemyPosition.Value;
                closestEnemyDistance = math.distance(playerPosition.Value, enemyPosition.Value);
            }
        }).Run();



        // Raycast to enemy

        

        int dataCount = playerQuery.CalculateEntityCount();
        LayerMask layerMask = ~LayerMask.GetMask("Projectile");

        NativeArray<UnityEngine.RaycastHit> results = new NativeArray<UnityEngine.RaycastHit>(dataCount, Allocator.TempJob);
        NativeArray<RaycastCommand> raycastCommand = new NativeArray<RaycastCommand>(dataCount, Allocator.TempJob);
        NativeArray<bool> hit = new NativeArray<bool>(dataCount, Allocator.TempJob);

        JobHandle jobHandle = Entities.WithStoreEntityQueryInField(ref playerQuery)
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation, in PlayerTag playerTag) =>
            {
                Vector3 origin = translation.Value;
                Vector3 direction = closestTargetPosition - translation.Value;

                raycastCommand[entityInQueryIndex] = new RaycastCommand(origin, direction, layerMask);


            }).ScheduleParallel(Dependency);

        JobHandle handle = RaycastCommand.ScheduleBatch(raycastCommand, results, dataCount, jobHandle);

        handle.Complete();

        for (int i = 0; i < dataCount; i++)
        {
            if (results[i].collider == null)
            {
                hit[i] = false;
            }
            else
            {
                hit[i] = true;
            }
        }

        // chesk if player has vision 
         Entities.WithStructuralChanges()
            .ForEach((Entity entity,
            int entityInQueryIndex,
            ref PlayerTag player,
            ref Translation translation,
            ref Rotation rotation,
            ref MoveData moveData,
            ref PhysicsVelocity physics,
            ref PlayerData playerData) =>
        {


            if (!hit[entityInQueryIndex])
            {
                // player sees the enemy

                if (moveData.direction.Equals(float3.zero) && closestEnemyDistance != 0 && closestEnemyDistance <= playerData.PlayerRange)
                {
                    physics.Linear.x = 0;
                    physics.Linear.z = 0;
                    playerData.StandStillTimer += deltaTime;

                    if (playerData.WeaponCooldown <= 0 && playerData.StandStillTimer >= 0.18f)
                    {
                        playerData.WeaponCooldown = playerData.WeaponBaseCooldown;

                        Entity instance = EntityManager.Instantiate(projectile);

                        float3 offset = new float3(0, 0, 1);

                        EntityManager.SetComponentData(instance, new Translation
                        {
                            Value = new float3(translation.Value) + math.mul(rotation.Value, offset)
                        });
                        EntityManager.SetComponentData(instance, new Rotation
                        {
                            Value = rotation.Value
                        });
                        EntityManager.SetComponentData(instance, new BulletData
                        {
                            Damage = playerData.WeaponBaseDamage
                        });
                    }

                }
                else
                {
                    playerData.StandStillTimer = 0;

                }

            }
            playerData.WeaponCooldown -= deltaTime;

        }).Run();

        results.Dispose();
        raycastCommand.Dispose();
        hit.Dispose();
    }
}
