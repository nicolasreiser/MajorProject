using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

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
                ForEach((Entity entity, DynamicBuffer<Child> childrenFromEntity) =>
                {
                    if (!childrenFromEntity.IsEmpty)
                    {
                        List<Entity> entities = GetListOfEntities(entity, childrenFromEntity);
                        foreach (var item in entities)
                        {
                            EntityManager.DestroyEntity(item);
                            Debug.Log("Deleted entity");
                        }
                    }
                }).Run();


            levelDataComponent.CleanupObstacles = true;
          

            EntityManager.SetComponentData(query.GetSingletonEntity(),levelDataComponent);
            Debug.Log("Set Cleanup to true");
          

        }

    }
    List<Entity> GetListOfEntities(Entity entity, DynamicBuffer<Child> childrenFromEntity)
    {
        // Create an empty list of Entities
        List<Entity> entities = new List<Entity>();
        BufferFromEntity<Child> childBuffer = this.GetBufferFromEntity<Child>(true);

        // Iterate over possible children
        foreach (var child in childrenFromEntity)
        {
            var childEntity = child.Value;
            bool hasChild = childBuffer.HasComponent(childEntity);
            // If a  child Entity is found recursively use this method to find other children
            if (hasChild)
            {
                var childbuffer = EntityManager.GetBuffer<Child>(childEntity);
                List<Entity> childEntitiesList = GetListOfEntities(childEntity, childbuffer);
                // Add the entities found to the main Entity list
                foreach (var item in childEntitiesList)
                {
                    entities.Add(item);
                }
            }
            else
            {
                entities.Add(childEntity);
            }
        }
        entities.Add(entity);
        // Return the list of child Entities
        return entities;
    }
}
