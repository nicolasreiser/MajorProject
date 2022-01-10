using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Jobs;

public class PathFindingSystem : SystemBase
{

    private EndSimulationEntityCommandBufferSystem ecb;

    private EntityQuery playerQuery;
    private EntityQuery enemyQuery;
    PhysicsWorld physicsWorld;
    EntityManager entityManager;

    protected override void OnCreate()
    {
        base.OnCreate();

        ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        physicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

    }

    protected override void OnUpdate()
    {
        PauseManagement pm = PauseManagement.Instance;

        if (pm != null)
        {
            if (pm.IsPaused)
            {
                return;
            }
        }
        var commandBuffer = ecb.CreateCommandBuffer().AsParallelWriter();

        playerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<PlayerTag>() }
        });

        var player = playerQuery.CalculateChunkCount();

        //get player position;

        float3 playerposition = float3.zero;

        if (player > 0)
        {
            
            Entities.WithAll<PlayerTag>()
                .WithStoreEntityQueryInField(ref playerQuery)
                .ForEach((Entity entity, ref Translation transform) =>
                {
                    
                    playerposition = transform.Value;

                }).Run();
        }

        //set enemy query

        enemyQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<EnemyTag>(), ComponentType.ReadOnly<PathfindState>() }
        });

        // calculate distance

        Entities.
            ForEach((Entity entity,
            int entityInQueryIndex,
            ref EnemyTag enemy,
            ref Translation transform,
            ref PathfindState pathfind) =>
            {
                if (!playerposition.Equals(float3.zero))
                {
                    float distance = math.distance(playerposition, transform.Value);
                    pathfind.PlayerDistance = distance;

                }
            }).ScheduleParallel();

        //check to exit current state

        // Raycast for player

        int dataCount = enemyQuery.CalculateEntityCount();
        LayerMask layerMask = ~LayerMask.GetMask("Projectile");


        NativeArray<UnityEngine.RaycastHit> results = new NativeArray<UnityEngine.RaycastHit>(dataCount, Allocator.TempJob);
        NativeArray<RaycastCommand> raycastCommand = new NativeArray<RaycastCommand>(dataCount, Allocator.TempJob);
        NativeArray<bool> hit = new NativeArray<bool>(dataCount, Allocator.TempJob);

        JobHandle jobHandle = Entities.WithStoreEntityQueryInField(ref enemyQuery).ForEach((Entity entity, int entityInQueryIndex, in Translation translation, in PathfindState pathfindState) =>
        {
            Vector3 origin = translation.Value;
            Vector3 direction = playerposition - translation.Value;
            raycastCommand[entityInQueryIndex] = new RaycastCommand(origin, direction/*,layerMask*/);

        }).ScheduleParallel(Dependency);

        JobHandle handle = RaycastCommand.ScheduleBatch(raycastCommand, results, dataCount, jobHandle);

        handle.Complete();

        for(int i = 0; i < dataCount; i++)
        {
            if(results[i].collider == null)
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
            ref EnemyData enemyData,
            ref Translation transform,
            ref PathfindState pathfind) =>
        {
            //TODO If in attack range change state
            //TODO If player too far change state

            // enemy alive 
            if (enemyData.CurrentHealth <= 0)
            {
                commandBuffer.AddComponent<FsmStateChanged>(entityInQueryIndex, entity);
                commandBuffer.SetComponent(entityInQueryIndex, entity, new FsmStateChanged
                {
                    from = FsmState.Pathfind,
                    to = FsmState.Death
                });
                return;
            }
            //player too far
            if (pathfind.PlayerDistance != 0 && pathfind.PlayerDistance > pathfind.PlayerOurOfRangeDistance)
            {
                commandBuffer.AddComponent<FsmStateChanged>(entityInQueryIndex, entity);
                commandBuffer.SetComponent(entityInQueryIndex, entity, new FsmStateChanged
                {
                    from = FsmState.Pathfind,
                    to = FsmState.Idle
                });
            }

            if (hit[entityInQueryIndex])
            {
                // continue pathing
                Debug.Log("I dont see the player, Pathfind");
            }
            else
            {
               

                if (pathfind.PlayerDistance != 0 && pathfind.EnemyAttackRange > pathfind.PlayerDistance)
                {
                    commandBuffer.AddComponent<FsmStateChanged>(entityInQueryIndex, entity);
                    commandBuffer.SetComponent(entityInQueryIndex, entity, new FsmStateChanged
                    {
                        from = FsmState.Pathfind,
                        to = FsmState.Attack
                    });
                }
            }



        }).ScheduleParallel(handle);
        enemyvision.Complete();

        ecb.AddJobHandleForProducer(Dependency);

        results.Dispose();
        raycastCommand.Dispose();
        hit.Dispose();
    }



}
