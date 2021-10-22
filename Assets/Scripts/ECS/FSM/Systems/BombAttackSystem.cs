using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

public class BombAttackSystem : SystemBase
{
    private EntityQuery playerQuery;
    private EntityQuery enemyQuery;

    protected override void OnCreate()
    {
        base.OnCreate();


        playerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<PlayerTag>() }
        });
    }
    protected override void OnUpdate()
    {
        bool isPlayerHit = false;
        int damageToDeal = 0;
        float deltaTime = Time.DeltaTime;
        // get player position

        float3 playerposition = float3.zero;


        int player = playerQuery.CalculateEntityCount();

        if (player > 0)
        {
            Entities
                .WithStoreEntityQueryInField(ref playerQuery)
                .WithNone<EnemyTag>()
                .ForEach((Entity entity,
                ref Translation transform) =>
                {
                    playerposition = transform.Value;



                }).Run();



            // check explosion radius

            Entities
                .WithAll<EnemyTag, AttackState,BombUnitTag>()
                .ForEach((Entity entity,
                ref Translation transform,
                ref AttackState attackState,
                ref PhysicsVelocity physics,
                ref EnemyData enemyData) =>
                {
                    physics.Linear = float3.zero;

                    float distance = math.distance(playerposition, transform.Value);

                    attackState.CurrentShootCooldown -= deltaTime;

                    if(enemyData.Health != 0 && attackState.CurrentShootCooldown <= 0)
                    {
                        if(distance <= attackState.EnemyAttackRange)
                        {
                            isPlayerHit = true;
                            damageToDeal = attackState.DamageToDeal;
                            enemyData.Health = 0;
                        }
                    }
                }).Run();

            //checked if in explosion radius

            Entities.WithStoreEntityQueryInField(ref playerQuery)
                .WithNone<EnemyTag>().
                ForEach((Entity entity,
                ref PlayerData playerData) =>
                {
                    if(isPlayerHit)
                    {
                        playerData.Health -= damageToDeal;
                        Debug.Log("Damage dealt");
                        isPlayerHit = false;
                    }

                }).Run();


        }



        // change to death state


    }
}
