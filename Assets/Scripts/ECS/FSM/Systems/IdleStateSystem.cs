using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class IdleStateSystem : SystemBase
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

        if(player > 0)
        {
            Entities
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
            ref IdleState idleState) =>
        {
            if(!playerposition.Equals(float3.zero))
            {
                float distance = math.distance(playerposition, transform.Value);

                idleState.PlayerDistance = distance;
            }
        }).ScheduleParallel();

        //check to exit current state

        Entities.
            ForEach((Entity entity,
            int entityInQueryIndex,
            ref EnemyTag enemy,
            ref Translation transform,
            ref IdleState idleState) =>
            {
               if (idleState.PlayerDistance != 0 && idleState.PlayerDistance < idleState.MaxPlayerDistance)
               {
                    commandBuffer.AddComponent<FsmStateChanged>(entityInQueryIndex, entity);
                    commandBuffer.SetComponent(entityInQueryIndex, entity, new FsmStateChanged
                    {
                        from = FsmState.Idle,
                        to = FsmState.Pathfind
                    });
                }
            }).ScheduleParallel();

        ecb.AddJobHandleForProducer(Dependency);
    }



}
