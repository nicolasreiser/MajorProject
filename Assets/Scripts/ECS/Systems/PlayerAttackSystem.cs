using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;


public class PlayerAttackSystem : SystemBase
{

    
    protected override void OnUpdate()
    {
        Entity projectile = Entity.Null;
        float deltaTime = Time.DeltaTime;


        Entities.ForEach((PrefabEntityStorage prefabs) =>
        {
            projectile = prefabs.PlayerProjectile;
        }).Run();


        Entities.
            WithStructuralChanges().
            WithAll<PlayerTag>().
            ForEach((Entity entity, ref PlayerData playerData, ref Translation translation, ref Rotation rotation, ref MoveData moveData,ref  PhysicsVelocity physics) =>
            {
                if (moveData.direction.Equals(float3.zero))
                {
                    physics.Linear = float3.zero;
                    playerData.StandStillTimer += deltaTime;

                    if (playerData.WeaponCooldown <= 0 && playerData.StandStillTimer >= 0.18f)
                    {
                        playerData.WeaponCooldown = playerData.WeaponBaseCooldown;

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

                        Debug.Log("Shooting entity");
                    }

                }
                else
                {
                playerData.StandStillTimer = 0;

                }

                playerData.WeaponCooldown -= deltaTime;
            }).Run();
    }
}
