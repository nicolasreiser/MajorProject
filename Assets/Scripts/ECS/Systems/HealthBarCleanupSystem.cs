using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// system removing heallthbars from dead enemies
public class HealthBarCleanupSystem : SystemBase
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
        if(objectPooler == null)
        {
            objectPooler = GameObject.FindObjectOfType<ObjectPooler>();

        }
        var commandBuffer = ecb.CreateCommandBuffer();

        // return healthbar to pbject pooler
        Entities.WithoutBurst().
            WithNone<PausedTag>().
            ForEach((Entity entity, HealthBarData healthBarData, in LifetimeData lifetimeData) =>
            {

                GameObject obj = healthBarData.slider.gameObject;
                objectPooler.ReturnObjectToPool(obj);
                //commandBuffer.DestroyEntity(entity);

            }).Run();

        // remove components
        Entities.
            WithoutBurst().
            WithNone<EnemyData>().
            ForEach((Entity entity, HealthBarData healthBarData) =>
            {
                commandBuffer.RemoveComponent<HealthBarData>(entity);
                commandBuffer.DestroyEntity(entity);

            }).Run();
    }
}
