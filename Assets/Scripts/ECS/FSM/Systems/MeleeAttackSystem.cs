using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

// sub attack system for melee enemies
public class MeleeAttackSystem : SystemBase
{
    private EntityQuery playerQuery;

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

        playerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<PlayerTag>() }
        });

        int player = playerQuery.CalculateEntityCount();

        if (player > 0)
        {
            Entities
                .WithStoreEntityQueryInField(ref playerQuery)
                .WithAll<PlayerTag>()
                .ForEach((Entity entity,
                ref Translation transform) =>
                {
                    playerposition = transform.Value;
                }).Run();

            //calculate player distance

            Entities
               .WithAll<EnemyTag, AttackState, MeleeUnitTag>()
               .ForEach((Entity entity,
               ref Translation transform,
               ref AttackState attackState,
               ref PhysicsVelocity physics,
               ref EnemyData enemyData) =>
               {
                   physics.Linear = float3.zero;

                   float distance = math.distance(playerposition, transform.Value);

                   attackState.CurrentShootCooldown -= deltaTime;

                   if (attackState.CurrentShootCooldown <= 0)
                   {
                       if (distance <= attackState.EnemyMaxAttackRange)
                       {

                           isPlayerHit = true;
                           damageToDeal = attackState.DamageToDeal;
                       }
                       attackState.CurrentShootCooldown = attackState.BaseShootCooldown;
                   }

               }).Run();

            // attack player

            Entities.WithStoreEntityQueryInField(ref playerQuery)
                .WithNone<EnemyTag>().
                ForEach((Entity entity,
                ref PlayerData playerData) =>
                {
                    if (isPlayerHit && !playerData.IsInvulnerable)
                    {
                        playerData.CurrentHealth -= damageToDeal;
                        playerData.OnHealthChange = true;

                        //Debug.Log("Damage dealt");
                        isPlayerHit = false;
                    }

                }).Run();

            // Spin animation

            Entities
                .WithoutBurst()
              .WithAll<EnemyTag, AttackState, BladeComponent>()
              .ForEach((Entity entity,
              ref BladeComponent blade ) =>
              {

                  // scale up blade
                  NonUniformScale s = EntityManager.GetComponentData<NonUniformScale>(blade.Blade);

                  if(s.Value.x < 200)
                  {
                      //Debug.Log("Scaling  up blade");
                      s.Value += deltaTime * blade.ScaleSpeed * 50;
                      EntityManager.SetComponentData<NonUniformScale>(blade.Blade, s);
                  }

                  // rotate blade

                  Rotation r = EntityManager.GetComponentData<Rotation>(blade.Blade);

                  r.Value = math.mul(r.Value, quaternion.RotateZ(math.radians(blade.SpinSpeed * deltaTime * 50)));

                  EntityManager.SetComponentData<Rotation>(blade.Blade, r);

              }).Run();

            //Retract animation
            Entities
                .WithoutBurst()
              .WithAll<EnemyTag, BladeComponent>()
              .WithNone<AttackState>()
              .ForEach((Entity entity,
              ref BladeComponent blade) =>
              {

                  NonUniformScale s = EntityManager.GetComponentData<NonUniformScale>(blade.Blade);
                  // scale up blade


                  if (s.Value.x > 100)
                  {
                      //Debug.Log("Scaling down blade");
                      s.Value -= deltaTime * blade.ScaleSpeed * 50;
                      EntityManager.SetComponentData<NonUniformScale>(blade.Blade, s);
                  }


              }).Run();

        }

    }
}
