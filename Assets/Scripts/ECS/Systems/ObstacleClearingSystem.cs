using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

//[UpdateBefore(typeof (LevelManagerSystem))]
public class ObstacleClearingSystem : SystemBase
{
    protected override void OnUpdate()
    {

        EntityQuery query = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<LevelDataComponent>());

        if (query.IsEmpty)
            return;

        LevelDataComponent levelDataComponent = EntityManager.GetComponentData<LevelDataComponent>(query.GetSingletonEntity());

        if(levelDataComponent.ReadyForNextLevel && !levelDataComponent.CleanupObstacles)
        {
            Entities.
                WithStructuralChanges().
                WithAll<ToDestroyTag>().
                ForEach((Entity entity) =>
                {
                   
                    EntityManager.DestroyEntity(entity);
                }).Run();

            levelDataComponent.CleanupObstacles = true;
          

            EntityManager.SetComponentData(query.GetSingletonEntity(),levelDataComponent);
            Debug.Log("Set Clearup to true");
          

        }

    }
}
