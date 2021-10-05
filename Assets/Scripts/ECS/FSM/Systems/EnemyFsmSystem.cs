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

        //var count = enemyWithoutFsmQuery.CalculateChunkCount();


        var ecbConcurrent = commandBuffer.AsParallelWriter();

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref EnemyFiniteStateMachine fsm, in FsmStateChanged stateChanged) => 
        { 

            switch(stateChanged.from)
            {
                case FsmState.Idle:
                    ecbConcurrent.RemoveComponent<IdleState>(entityInQueryIndex, entity);
                    ecbConcurrent.SetComponent(entityInQueryIndex, entity, new IdleState
                    {
                        PlayerDistance = 0,
                        MaxPlayerDistance = 5
                    });
                    break;
                case FsmState.Attack:
                    ecbConcurrent.RemoveComponent<AttackState>(entityInQueryIndex, entity);
                    break;
                case FsmState.Pathfind:
                    ecbConcurrent.RemoveComponent<PathfindState>(entityInQueryIndex, entity);

                    break;
                case FsmState.Death:
                    break;
            }

            fsm.currentState = stateChanged.to;

            switch (stateChanged.to)
            {
                
                case FsmState.Idle:
                    ecbConcurrent.AddComponent<IdleState>(entityInQueryIndex, entity);
                    
                    break;
                case FsmState.Attack:
                    ecbConcurrent.AddComponent<AttackState>(entityInQueryIndex, entity);

                    break;
                case FsmState.Pathfind:
                    ecbConcurrent.AddComponent<PathfindState>(entityInQueryIndex, entity);

                    break;
                case FsmState.Death:
                    ecbConcurrent.AddComponent<DeathState>(entityInQueryIndex, entity);

                    break;
                
            }

            ecbConcurrent.RemoveComponent<FsmStateChanged>(entityInQueryIndex, entity);


        }).ScheduleParallel();

        ecb.AddJobHandleForProducer(Dependency);

    }
}
