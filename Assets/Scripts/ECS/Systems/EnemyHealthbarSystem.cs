using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class EnemyHealthbarSystem : SystemBase
{

    protected override void OnUpdate()
    {

        Entities.
            WithoutBurst().
            WithAll<EnemyTag>().
            ForEach((Entity entity, HealthBarData healthbarData, in EnemyData enemydata, in Translation transform) =>
            {
                
                if (enemydata.CurrentHealth == enemydata.BaseHealth)
                {
                    // hide healthbar
                    healthbarData.slider.enabled = false;
                }
                else
                {
                    healthbarData.slider.enabled = true;
                    healthbarData.slider.transform.position = transform.Value + healthbarData.Offset;
                    healthbarData.slider.maxValue = enemydata.BaseHealth;
                    healthbarData.slider.value = enemydata.CurrentHealth;

                }

            }).Run();
    }
}
