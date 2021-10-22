using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[UpdateAfter(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]

public class TimedDestroySystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref LifetimeData lifetimeData) => 
            {
                if(lifetimeData.ShouldDie)
                {
                    EntityManager.DestroyEntity(entity);
                    return;
                }
                lifetimeData.Lifetime -= deltaTime;
                if(lifetimeData.Lifetime <= 0)
                {
                    EntityManager.DestroyEntity(entity);
                }
            }).Run();
    }
}
