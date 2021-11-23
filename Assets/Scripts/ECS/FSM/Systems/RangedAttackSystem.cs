using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;


public class RangedAttackSystem : SystemBase
{
    //Entity projectile;

    protected override void OnCreate()
    {
        base.OnCreate();
        
    }
    protected override void OnUpdate()
    {
        Entity projectile = Entity.Null;
        float deltaTime = Time.DeltaTime;


        Entities.ForEach((in PrefabEntityStorage prefabs) =>
        {
            projectile = prefabs.EnemyProjectile;

        }).Run();


        Entities
            .WithStructuralChanges()
            .WithAll<EnemyTag, AttackState, RangedUnitTag>()
            .ForEach((Entity entity, ref Translation translation,ref Rotation rotation , ref AttackState attackState, ref PhysicsVelocity physics) =>
            {
                physics.Linear = float3.zero;
                if(attackState.CurrentShootCooldown <= 0)
                {
                    attackState.CurrentShootCooldown = attackState.BaseShootCooldown;

                    Entity instance = EntityManager.Instantiate(projectile);

                    float3 offset = new float3(0, 0, 1);

                    EntityManager.SetComponentData(instance, new Translation
                    {
                        Value = new float3(translation.Value) + math.mul(rotation.Value, offset)
                    });
                    EntityManager.SetComponentData(instance, new Rotation
                    {
                        Value = rotation.Value
                    });
                    EntityManager.SetComponentData(instance, new BulletData
                    {
                        Origin = BulletOrigin.Enemy,
                        Damage = attackState.DamageToDeal
                    }) ;

                    //Debug.Log("Shooting entity");
                }
                attackState.CurrentShootCooldown -= deltaTime;
            }).Run();
    }

}
