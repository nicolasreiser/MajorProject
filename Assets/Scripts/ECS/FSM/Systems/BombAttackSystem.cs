using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

// sub attack system specific for the bomb enemies
public class BombAttackSystem : SystemBase
{
    private EntityQuery playerQuery;

    protected override void OnCreate()
    {
        base.OnCreate();


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

        bool isPlayerHit = false;
        int damageToDeal = 0;
        float deltaTime = Time.DeltaTime;

        // get player position

        float3 playerposition = float3.zero;

        //playerQuery = GetEntityQuery(new EntityQueryDesc
        //{
        //    All = new ComponentType[] { ComponentType.ReadOnly<PlayerTag>() }
        //});

        //int player = playerQuery.CalculateEntityCount();

        //if (player > 0)
        {
            Entities
                .WithNone<EnemyTag>()
                .WithAll<PlayerData>()
                .ForEach((Entity entity,
                ref Translation transform) =>
                {
                    playerposition = transform.Value;



                }).Run();


            


            // check explosion radius

            Entities
                .WithStructuralChanges()
                .WithAll<EnemyTag, AttackState,BombUnitTag>()
                .ForEach((Entity entity,
                ref Translation transform,
                ref AttackState attackState,
                ref PhysicsVelocity physics,
                ref EnemyData enemyData,
                ref Translation translation) =>
                {
                    physics.Linear = float3.zero;

                    float distance = math.distance(playerposition, transform.Value);

                    attackState.CurrentShootCooldown -= deltaTime;

                    if(enemyData.CurrentHealth != 0 && attackState.CurrentShootCooldown <= 0)
                    {
                        if(distance <= attackState.EnemyMaxAttackRange)
                        {

                            isPlayerHit = true;
                            damageToDeal = attackState.DamageToDeal;
                            enemyData.CurrentHealth = 0;

                            
                        }
                    }
                }).Run();


            
            //checked if in explosion radius

            Entities.WithStoreEntityQueryInField(ref playerQuery)
                .WithNone<EnemyTag>().
                ForEach((Entity entity,
                ref PlayerData playerData) =>
                {

                    if (isPlayerHit && !playerData.IsInvulnerable)
                    {
                        playerData.CurrentHealth -= damageToDeal;
                        playerData.OnHealthChange = true;

                        Debug.Log("Damage dealt");
                        isPlayerHit = false;
                    }

                }).Run();

            

            
        }
    }
}
