using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

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
        DynamicBuffer<EnemyDataContainer> tempEnemyDataContainer = new DynamicBuffer<EnemyDataContainer>();


        Entities.WithoutBurst().ForEach((in DynamicBuffer<EnemyDataContainer> enemyDataContainer) =>
        {
            tempEnemyDataContainer = enemyDataContainer;
        }
        ).Run();

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref EnemyFiniteStateMachine fsm, in FsmStateChanged stateChanged, in EnemyTypeData enemyTypeData) => 
        {
            int type = ((int)enemyTypeData.enemyType);
            int  enemyAttackRange = tempEnemyDataContainer[type].Range;
            int  enemyMaxAttackRange = tempEnemyDataContainer[type].MaxRange;
            int  enemyDetectionRange = tempEnemyDataContainer[type].DetectionRange;
            float   enemyWeaponCooldown = tempEnemyDataContainer[type].Cooldown;
            int  enemyDamageToDeal = tempEnemyDataContainer[type].Damage;
          
            switch (stateChanged.from)
            {
                case FsmState.Idle:
                    ecbConcurrent.RemoveComponent<IdleState> (entity);

                    //ecbConcurrent.RemoveComponent<IdleState>(entityInQueryIndex, entity);
                    
                    break;
                case FsmState.Attack:
                    ecbConcurrent.RemoveComponent<AttackState>( entity);

                    //ecbConcurrent.RemoveComponent<AttackState>(entityInQueryIndex, entity);
                    break;
                case FsmState.Pathfind:

                    ecbConcurrent.RemoveComponent<PathfindState>( entity);
                    ecbConcurrent.RemoveComponent<PathfindingParams>( entity);
                    ecbConcurrent.RemoveComponent<PathFollow>( entity);
                    ecbConcurrent.RemoveComponent<PathPosition>( entity);

                    //ecbConcurrent.RemoveComponent<PathfindState>(entityInQueryIndex, entity);
                    //ecbConcurrent.RemoveComponent<PathfindingParams>(entityInQueryIndex, entity);
                    //ecbConcurrent.RemoveComponent<PathFollow>(entityInQueryIndex, entity);
                    //ecbConcurrent.RemoveComponent<PathPosition>(entityInQueryIndex, entity);


                    break;
                case FsmState.Death:
                    break;
            }

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
                    //ecbConcurrent.AddComponent<IdleState>(entityInQueryIndex, entity);
                    //ecbConcurrent.SetComponent(entityInQueryIndex, entity, new IdleState
                    //{
                    //    // to change
                    //    PlayerDistance = 0,
                    //    EnemyDetectionRange = enemyDetectionRange
                    //});

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
                    //ecbConcurrent.AddComponent<AttackState>(entityInQueryIndex, entity);
                    //ecbConcurrent.SetComponent(entityInQueryIndex, entity, new AttackState
                    //{
                    //    EnemyAttackRange = enemyAttackRange,
                    //    EnemyMaxAttackRange = enemyMaxAttackRange,
                    //    BaseShootCooldown = enemyWeaponCooldown,
                    //    CurrentShootCooldown = enemyWeaponCooldown,
                    //    DamageToDeal = enemyDamageToDeal
                    //});

                    //Debug.Log("Changed to Attack State");

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
                    // ecbConcurrent.AddBuffer<PathPosition>(entityInQueryIndex, entity);
                    // ecbConcurrent.AddComponent<PathFollow>(entityInQueryIndex, entity);
                    // ecbConcurrent.SetComponent(entityInQueryIndex, entity, new PathFollow
                    // {
                    //     pathIndex = -1
                    // });
                    // ecbConcurrent.AddComponent<PathfindState>(entityInQueryIndex, entity);
                    // ecbConcurrent.SetComponent(entityInQueryIndex, entity, new PathfindState
                    // {
                    //     PlayerOurOfRangeDistance = 15,
                    //     EnemyAttackRange = enemyAttackRange,
                    //     PathfindCooldown = 0
                    //     
                    // });
                    break;
                case FsmState.Death:

                    ecbConcurrent.AddComponent<DeathState>( entity);
                    ecbConcurrent.AddComponent<LifetimeData>( entity);
                    ecbConcurrent.SetComponent( entity, new LifetimeData
                    {
                        Lifetime = 0.1f,
                        ShouldDie = false
                    }); 

                    //ecbConcurrent.AddComponent<DeathState>(entityInQueryIndex, entity);
                    //ecbConcurrent.AddComponent<LifetimeData>(entityInQueryIndex, entity);
                    //ecbConcurrent.SetComponent(entityInQueryIndex, entity, new LifetimeData
                    //{
                    //    Lifetime = 0.1f,
                    //    ShouldDie = false
                    //});

                    break;
                
            }

            ecbConcurrent.RemoveComponent<FsmStateChanged>( entity);

            //ecbConcurrent.RemoveComponent<FsmStateChanged>(entityInQueryIndex, entity);


        }).Run();

        ecb.AddJobHandleForProducer(Dependency);

    }
}
