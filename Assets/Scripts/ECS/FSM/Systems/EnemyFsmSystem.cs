using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

// base fsm system controlling the states of the enemies

public class EnemyFsmSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecb;


    protected override void OnCreate()
    {
        base.OnCreate();

        ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

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

        var commandBuffer = ecb.CreateCommandBuffer();

        var ecbConcurrent = commandBuffer;

        // get enemy data dynamic buffer
        DynamicBuffer<EnemyDataContainer> tempEnemyDataContainer = new DynamicBuffer<EnemyDataContainer>();

        Entities.WithoutBurst().ForEach((in DynamicBuffer<EnemyDataContainer> enemyDataContainer) =>
        {
            tempEnemyDataContainer = enemyDataContainer;
        }
        ).Run();

        // iterate over entities with a FSMStateChanged component

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref EnemyFiniteStateMachine fsm, in FsmStateChanged stateChanged, in EnemyTypeData enemyTypeData) => 
        {
            // get enemy type and store essential data
            int type = ((int)enemyTypeData.enemyType);
            int  enemyAttackRange = tempEnemyDataContainer[type].Range;
            int  enemyMaxAttackRange = tempEnemyDataContainer[type].MaxRange;
            int  enemyDetectionRange = tempEnemyDataContainer[type].DetectionRange;
            float   enemyWeaponCooldown = tempEnemyDataContainer[type].Cooldown;
            int  enemyDamageToDeal = tempEnemyDataContainer[type].Damage;
          
            // remove old state component
            switch (stateChanged.from)
            {
                case FsmState.Idle:
                    ecbConcurrent.RemoveComponent<IdleState> (entity);
                    break;
                case FsmState.Attack:
                    ecbConcurrent.RemoveComponent<AttackState>( entity);
                    break;
                case FsmState.Pathfind:

                    ecbConcurrent.RemoveComponent<PathfindState>( entity);
                    ecbConcurrent.RemoveComponent<PathfindingParams>( entity);
                    ecbConcurrent.RemoveComponent<PathFollow>( entity);
                    ecbConcurrent.RemoveComponent<PathPosition>( entity);
                    break;
                case FsmState.Death:
                    break;
            }

            // add new state component
            fsm.currentState = stateChanged.to;

            switch (stateChanged.to)
            {
                case FsmState.Idle:

                    ecbConcurrent.AddComponent<IdleState>(entity);
                    ecbConcurrent.SetComponent( entity, new IdleState
                    {
                        PlayerDistance = 0,
                        EnemyDetectionRange = enemyDetectionRange
                    });
                    break;
                case FsmState.Attack:

                    ecbConcurrent.AddComponent<AttackState>(entity);
                    ecbConcurrent.SetComponent( entity, new AttackState
                    {
                        EnemyAttackRange = enemyAttackRange,
                        EnemyMaxAttackRange = enemyMaxAttackRange,
                        BaseShootCooldown = enemyWeaponCooldown,
                        CurrentShootCooldown = enemyWeaponCooldown,
                        DamageToDeal = enemyDamageToDeal
                    });
                    break;
                case FsmState.Pathfind:

                    ecbConcurrent.AddBuffer<PathPosition>( entity);
                    ecbConcurrent.AddComponent<PathFollow>( entity);
                    ecbConcurrent.SetComponent( entity, new PathFollow
                    {
                        pathIndex = -1
                    });
                    ecbConcurrent.AddComponent<PathfindState>( entity);
                    ecbConcurrent.SetComponent( entity, new PathfindState
                    {
                        PlayerOurOfRangeDistance = 15,
                        EnemyAttackRange = enemyAttackRange,
                        PathfindCooldown = 0
                    });
                    break;
                case FsmState.Death:

                    ecbConcurrent.AddComponent<DeathState>( entity);
                    ecbConcurrent.AddComponent<LifetimeData>( entity);
                    ecbConcurrent.SetComponent( entity, new LifetimeData
                    {
                        Lifetime = 0.1f,
                        ShouldDie = false
                    }); 
                    break;
            }
            ecbConcurrent.RemoveComponent<FsmStateChanged>( entity);
        }).Run();

        ecb.AddJobHandleForProducer(Dependency);
    }
}
