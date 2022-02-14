using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public class ScatterShotSystem : SystemBase
{
        EntityQuery query;

    protected override void OnUpdate()
    {

        // Get a list of enemies
        query = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<EnemyTag>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadWrite<EnemyData>() }
        });

        NativeList<Entity> enemies = new NativeList<Entity>(Allocator.Temp);

        Entities.WithStoreEntityQueryInField(ref query).
            ForEach((Entity entity, ref EnemyData enemyData, in Translation translation ) =>
            {
                enemies.Add(entity);
            }).Run();


        float deltaTime = Time.DeltaTime;

        //Get scatter effect
        Entity explosion = Entity.Null;
        Entities.
            WithNone<PausedTag>().
            ForEach((in PrefabEntityStorage prefabs) =>
            {
                explosion = prefabs.EnemyScatter;
            }).Run();

        // iterate over enemies and damage the ones in range
        Entities.
            WithoutBurst().
            WithStructuralChanges().
            WithAll<EnemyTag>().
            WithNone<PausedTag>().
            ForEach((Entity entity, ref EnemyData enemyData, in Translation translation) =>
            {
                if(enemyData.ScatterShot && enemyData.ScatterShotCooldown <= 0)
                {
                    //Debug.Log("AOE Dmg");
                    foreach(Entity e in enemies)
                    {
                        Translation t = EntityManager.GetComponentData<Translation>(e);

                        float distance = math.distance(translation.Value, t.Value);

                        if (distance <= 3)
                        {
                            EnemyData eD = EntityManager.GetComponentData<EnemyData>(e);
                            eD.CurrentHealth -= enemyData.ScatterShotDamage;
                            if (eD.CurrentHealth < 0)
                            {
                                eD.CurrentHealth = 0;
                            }
                            EntityManager.SetComponentData(e, eD);
                        }
                    }
                    // Particles

                    var instance = EntityManager.Instantiate(explosion);

                    EntityManager.SetComponentData(instance, new Translation
                    {
                        Value = new float3(translation.Value)
                    });

                    enemyData.ScatterShot = false;


                }
                enemyData.ScatterShotCooldown -= deltaTime;


            }).Run();
    }

}
