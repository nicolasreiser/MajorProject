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
    private Canvas uiCanvas;
    private Camera uiCamera;

    private EndSimulationEntityCommandBufferSystem ecb;
    protected override void OnCreate()
    {
        objectPooler = GameObject.FindObjectOfType<ObjectPooler>();

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

        if(uiCanvas == null)
        {
            GetData();
        }


        Entities.WithoutBurst()
            .WithAll<EnemyData>()
            .WithNone<HealthBarData>()
            .ForEach((Entity entity) =>
            {
                HealthBarData hbd = new HealthBarData();
                hbd.camera = uiCamera;
                hbd.canvas = uiCanvas;
                hbd.Offset = new float3(0, 1, 1);

                commandBuffer.AddComponent(entity, hbd);

            }).Run();


        Entities.WithoutBurst()
            .WithAll<HealthBarData>()
            .ForEach((Entity entity , HealthBarData healthBarData, in EnemyData enemyData) =>
            {
                if(enemyData.CurrentHealth != enemyData.BaseHealth && healthBarData.slider == null)
                {
                    GameObject h = objectPooler.getPooledObject();
                    healthBarData.slider = h.GetComponent<Slider>();
                    healthBarData.camera = uiCamera;
                    healthBarData.canvas = uiCanvas;
                }
            }).Run();


        Entities.
            WithoutBurst().
            WithAll<EnemyTag, HealthBarData>().
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

    private void GetData()
    {

        Entities.WithoutBurst().
            ForEach((Entity entity, MonobehaviourStorageComponent storage ) =>
            {
                uiCanvas = storage.UICanvas;
                uiCamera = storage.UICamera;
            }).Run();
    }
}
