using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PathFindingSystem : SystemBase
{

    private EndSimulationEntityCommandBufferSystem ecb;

    private EntityQuery playerQuery;

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
            
            Entities.WithNone<EnemyTag>()
                .WithStoreEntityQueryInField(ref playerQuery)
                .ForEach((Entity entity, ref Translation transform) =>
                {
                    
                    playerposition = transform.Value;

                }).Run();
        }

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

        Entities.
            ForEach((Entity entity,
            int entityInQueryIndex,
            ref EnemyTag enemy,
            ref Translation transform,
            ref PathfindState pathfind) =>
            {
                //TODO If in attack range change state
                //TODO If player too far change state

                //player too far
                if (pathfind.PlayerDistance != 0 && pathfind.PlayerDistance > pathfind.PlayerOurOfRangeDistance)
                {
                    Debug.Log("Changing State");
                    commandBuffer.AddComponent<FsmStateChanged>(entityInQueryIndex, entity);
                    commandBuffer.SetComponent(entityInQueryIndex, entity, new FsmStateChanged
                    {
                        from = FsmState.Pathfind,
                        to = FsmState.Idle
                    });
                }

                //player in attack range
                //if(pathfind.PlayerDistance != 0 && pathfind.EnemyAttackRange > pathfind.PlayerDistance)
                //{
                //    commandBuffer.AddComponent<FsmStateChanged>(entityInQueryIndex, entity);
                //    commandBuffer.SetComponent(entityInQueryIndex, entity, new FsmStateChanged
                //    {
                //        from = FsmState.Pathfind,
                //        to = FsmState.Attack
                //    });
                //}

            }).ScheduleParallel();

        ecb.AddJobHandleForProducer(Dependency);
    }



}
