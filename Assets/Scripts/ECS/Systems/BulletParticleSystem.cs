using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

// system creating particle systems when the bullet hits an obstacle

[UpdateBefore(typeof (TimedDestroySystem))]
[UpdateAfter(typeof (BulletCollisionEventSystem))]

public class BulletParticleSystem : SystemBase
{
    Entity playerBulletExplosion;
    Entity enemyBulletExplosion;
    protected override void OnCreate()
    {
        playerBulletExplosion = Entity.Null;
        enemyBulletExplosion = Entity.Null;
    }

    protected override void OnUpdate()
    {
        playerBulletExplosion = Entity.Null;
        enemyBulletExplosion = Entity.Null;

        //Get explosion effect
        if (playerBulletExplosion == Entity.Null)
        {
            Entities.
                WithoutBurst().
                WithNone<PausedTag>().
                ForEach((in PrefabEntityStorage prefabs) =>
                {
                    playerBulletExplosion = prefabs.PlayerBulletExplosion;
                    enemyBulletExplosion = prefabs.EnemyBulletExplosion;
                }).Run();
        }

        // iterate over the entities
        Entities.
            WithStructuralChanges().
            WithNone<PausedTag>().
            ForEach((Entity entity, ref BulletData bulletData, ref Translation translation, ref LifetimeData lifetimeData) =>
        {
            // check if a particle effect should be instantiated
            if (bulletData.ParticleEffect)
            {
                // check origin of the bullet
                if(bulletData.Origin == BulletOrigin.Player)
                {
                    var instance = EntityManager.Instantiate(playerBulletExplosion);

                    EntityManager.SetComponentData(instance, new Translation
                    {
                        Value = translation.Value
                    });
                }
                else
                {
                    var instance = EntityManager.Instantiate(enemyBulletExplosion);

                    EntityManager.SetComponentData(instance, new Translation
                    {
                        Value = translation.Value
                    });
                }
                bulletData.ParticleEffect = false;
                bulletData.ShouldDestroy = true;
            }
        }).Run();


    }
}
