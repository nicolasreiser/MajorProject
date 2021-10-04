using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class EnemyFsmSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecb;

    private EntityQuery enemyWithoutFsmQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        enemyWithoutFsmQuery = GetEntityQuery(new EntityQueryDesc
        {
            None = new ComponentType[] { ComponentType.ReadOnly<EnemyFiniteStateMachine>() },
            All = new ComponentType[] { ComponentType.ReadOnly<Cat>() }
        });
    }
    protected override void OnUpdate()
    {
        var commandBuffer = ecb.CreateCommandBuffer();

        var count = enemyWithoutFsmQuery.CalculateChunkCount();


        var ecbConcurrent = commandBuffer.AsParallelWriter();

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref EnemyFiniteStateMachine fsm, in FsmStateChanged stateChanged) => 
        { 

            switch(stateChanged.from)
            {
                case FsmState.Idle:
                    break;
                    ecbConcurrent.RemoveComponent<IdleState>(entityInQueryIndex, entity);
                case FsmState.Attack:
                    ecbConcurrent.RemoveComponent<AttackState>(entityInQueryIndex, entity);
                    break;
                case FsmState.Pathfind:
                    ecbConcurrent.RemoveComponent<PathfindState>(entityInQueryIndex, entity);

                    break;
                case FsmState.Death:
                    break;
            }


        }).ScheduleParallel();
    }
}
