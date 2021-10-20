using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TimedDestroySystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref LifetimeData lifetimeData) => 
            {
                lifetimeData.lifetimeData -= deltaTime;
                if(lifetimeData.lifetimeData <= 0)
                {
                    EntityManager.DestroyEntity(entity);
                }
            }).Run();
    }
}
