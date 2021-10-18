using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class EnemyFsmSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecb;

    //private EntityQuery enemyWithoutFsmQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        //enemyWithoutFsmQuery = GetEntityQuery(new EntityQueryDesc
        //{
        //    None = new ComponentType[] { ComponentType.ReadOnly<EnemyFiniteStateMachine>() },
        //    All = new ComponentType[] { ComponentType.ReadOnly<Cat>() }
        //});
    }
    protected override void OnUpdate()
    {
        var commandBuffer = ecb.CreateCommandBuffer();

        //var count = enemyWithoutFsmQuery.CalculateChunkCount();

        var ecbConcurrent = commandBuffer.AsParallelWriter();
        EnemyDataContainer tempEnemyDataContainer = new EnemyDataContainer();


        Entities.WithoutBurst().ForEach((EnemyDataContainer enemyDataContainer) =>
        {
            tempEnemyDataContainer = enemyDataContainer;
        }
        ).Run();

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref EnemyFiniteStateMachine fsm, in FsmStateChanged stateChanged, in EnemyTypeData enemyTypeData) => 
        {
            int enemyAttackRange = 0;

            switch (enemyTypeData.enemyType)
            {
                case EnemyType.Melee:
                    enemyAttackRange = tempEnemyDataContainer.meleeRange;
                    break;
                case EnemyType.Ranged:
                    enemyAttackRange = tempEnemyDataContainer.RangedRange;

                    break;
                case EnemyType.Bomb:
                    enemyAttackRange = tempEnemyDataContainer.BombRange;

                    break;
            }

            switch (stateChanged.from)
            {
                case FsmState.Idle:
                    ecbConcurrent.RemoveComponent<IdleState>(entityInQueryIndex, entity);
                    
                    break;
                case FsmState.Attack:
                    ecbConcurrent.RemoveComponent<AttackState>(entityInQueryIndex, entity);
                    break;
                case FsmState.Pathfind:
                    ecbConcurrent.RemoveComponent<PathfindState>(entityInQueryIndex, entity);
                    ecbConcurrent.RemoveComponent<PathfindingParams>(entityInQueryIndex, entity);
                    ecbConcurrent.RemoveComponent<PathFollow>(entityInQueryIndex, entity);
                    ecbConcurrent.RemoveComponent<PathfindingParams>(entityInQueryIndex, entity);

                    break;
                case FsmState.Death:
                    break;
            }

            fsm.currentState = stateChanged.to;

            switch (stateChanged.to)
            {
                
                case FsmState.Idle:
                    ecbConcurrent.AddComponent<IdleState>(entityInQueryIndex, entity);
                    ecbConcurrent.SetComponent(entityInQueryIndex, entity, new IdleState
                    {
                        PlayerDistance = 0,
                        MaxPlayerDistance = 10
                    });
                    Debug.Log("Changed to Idle State");

                    break;
                case FsmState.Attack:
                    ecbConcurrent.AddComponent<AttackState>(entityInQueryIndex, entity);
                    Debug.Log("Changed to Attack State");

                    break;
                case FsmState.Pathfind:
                    ecbConcurrent.AddComponent<PathFollow>(entityInQueryIndex, entity);
                    ecbConcurrent.SetComponent(entityInQueryIndex, entity, new PathFollow
                    {
                        pathIndex = -1
                    });
                    ecbConcurrent.AddComponent<PathfindState>(entityInQueryIndex, entity);
                    ecbConcurrent.SetComponent(entityInQueryIndex, entity, new PathfindState
                    {
                        PlayerOurOfRangeDistance = 15,
                        EnemyAttackRange = enemyAttackRange,
                        PathfindCooldown = 0
                        
                    });
                    //ecbConcurrent.AddComponent<PathfindingParams>(entityInQueryIndex, entity);
                    //ecbConcurrent.SetComponent(entityInQueryIndex, entity, new PathfindingParams
                    //{
                    //    // To change
                    //    startPosition = new int2(10, 10),
                    //    endPosition = new int2(15, 15)
                    //});
                    ecbConcurrent.AddBuffer<PathPosition>(entityInQueryIndex, entity);
                    Debug.Log("Changed to Pathfind State");
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
