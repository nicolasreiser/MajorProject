using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine.UI;

public class EnemyHealthbarSystem : SystemBase
{
    private ObjectPooler objectPooler;

    private EndSimulationEntityCommandBufferSystem ecb;
    protected override void OnCreate()
    {
        objectPooler = GameObject.FindObjectOfType<ObjectPooler>();
        ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

    }

    protected override void OnUpdate()
    {
        var commandBuffer = ecb.CreateCommandBuffer();

        Entities.WithoutBurst().
            ForEach((Entity entity,int entityInQueryIndex , HealthBarData healthBarData, in EnemyData enemyData) =>
            {
                if(enemyData.CurrentHealth != enemyData.BaseHealth && healthBarData.slider == null)
                {
                    GameObject h = objectPooler.getPooledObject();
                    healthBarData.slider = h.GetComponent<Slider>();
                }
            }).Run();


        Entities.
            WithoutBurst().
            WithAll<EnemyTag>().
            ForEach((Entity entity, HealthBarData healthbarData, in EnemyData enemydata, in Translation transform) =>
            {
                if (enemydata.CurrentHealth != enemydata.BaseHealth)
                {
                    healthbarData.slider.enabled = true;
                    healthbarData.slider.transform.position = transform.Value + healthbarData.Offset;
                    healthbarData.slider.transform.rotation = healthbarData.camera.transform.rotation;
                    healthbarData.slider.maxValue = enemydata.BaseHealth;
                    healthbarData.slider.value = enemydata.CurrentHealth;
                }

            }).Run();
    }
}
