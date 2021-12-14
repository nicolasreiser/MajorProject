using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

public class AttackStateSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecb;

    private EntityQuery playerQuery;
    private EntityQuery enemyQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        playerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<PlayerTag>() }
        });
    }

    protected override void OnUpdate()
    {
        var commandBuffer = ecb.CreateCommandBuffer().AsParallelWriter();

        var player = playerQuery.CalculateChunkCount();

        //get player position;

        float3 playerposition = float3.zero;

        if (player > 0)
        {
            Entities
                .WithStoreEntityQueryInField(ref playerQuery)
                .WithAll<PlayerTag>()
                .ForEach((Entity entity, ref Translation transform) =>
                {
                    playerposition = transform.Value;

                }).Run();
        }

        //set enemy query

        enemyQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<EnemyTag>(), ComponentType.ReadOnly<AttackState>() }
        });

        // calculate distance

        Entities.
            ForEach((Entity entity,
            int entityInQueryIndex,
            ref EnemyTag enemy,
            ref Translation transform,
            ref AttackState attackState) =>
            {
                if (!playerposition.Equals(float3.zero))
                {
                    float distance = math.distance(playerposition, transform.Value);

                    attackState.PlayerDistance = distance;
                }
            }).ScheduleParallel();

        //check to exit current state


        // Raycast to player

        int dataCount = enemyQuery.CalculateEntityCount();
        LayerMask layerMask =~ LayerMask.GetMask("Projectile");

        NativeArray<UnityEngine.RaycastHit> results = new NativeArray<UnityEngine.RaycastHit>(dataCount, Allocator.TempJob);
        NativeArray<RaycastCommand> raycastCommand = new NativeArray<RaycastCommand>(dataCount, Allocator.TempJob);
        NativeArray<bool> hit = new NativeArray<bool>(dataCount, Allocator.TempJob);

        JobHandle jobHandle = Entities.WithStoreEntityQueryInField(ref enemyQuery)
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation, in AttackState attackState) =>
        {
            Vector3 origin = translation.Value;
            Vector3 direction = playerposition - translation.Value;

            raycastCommand[entityInQueryIndex] = new RaycastCommand(origin, direction/*,layerMask*/);
            

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

        JobHandle enemyvision = Entities.ForEach((Entity entity,
            int entityInQueryIndex,
            ref EnemyTag enemy,
            ref Translation transform,
            ref AttackState attackState,
            ref EnemyData enemyData) =>
        {
            // check if enemy is alive

            if (enemyData.CurrentHealth <= 0)
            {
                commandBuffer.AddComponent<FsmStateChanged>(entityInQueryIndex, entity);
                commandBuffer.SetComponent(entityInQueryIndex, entity, new FsmStateChanged
                {
                    from = FsmState.Attack,
                    to = FsmState.Death
                });
                return;
            }
            //player too far
            if (attackState.PlayerDistance != 0 && attackState.PlayerDistance > attackState.EnemyMaxAttackRange)
            {
                commandBuffer.AddComponent<FsmStateChanged>(entityInQueryIndex, entity);
                commandBuffer.SetComponent(entityInQueryIndex, entity, new FsmStateChanged
                {
                    from = FsmState.Attack,
                    to = FsmState.Pathfind
                });
                return;
            }
            // player out of vision
            if (hit[entityInQueryIndex])
            {
                // change to Pathfinding

                commandBuffer.AddComponent<FsmStateChanged>(entityInQueryIndex, entity);
                commandBuffer.SetComponent(entityInQueryIndex, entity, new FsmStateChanged
                {
                    from = FsmState.Attack,
                    to = FsmState.Pathfind
                });
            }

        }).ScheduleParallel(handle);
        enemyvision.Complete();
        raycastCommand.Dispose();
        results.Dispose();
        hit.Dispose();

        ecb.AddJobHandleForProducer(Dependency);

    }

}
