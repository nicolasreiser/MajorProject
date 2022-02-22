using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

// system destroying entities

[UpdateAfter(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]
public class TimedDestroySystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecb;

    protected override void OnCreate()
    {
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

        float deltaTime = Time.DeltaTime;

        //var commandBuffer = ecb.CreateCommandBuffer().AsParallelWriter();

        Entities.WithoutBurst()
            .WithStructuralChanges()
            .WithNone<PausedTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref LifetimeData lifetimeData, ref DynamicBuffer<Child> childrenFromEntity) =>
            {
                if (lifetimeData.ShouldDie)
                {
                    //destroy entities with ShouldDie enables
                    if(!childrenFromEntity.IsEmpty)
                    {
                        List<Entity> entities = GetListOfEntities(entity, childrenFromEntity);
                        foreach (var item in entities)
                        {
                            EntityManager.DestroyEntity(item);
                        }
                    }
                    Debug.Log("Entity Destroyed");
                    return;
                }
                lifetimeData.Lifetime -= deltaTime;
                if (lifetimeData.Lifetime <= 0)
                {
                    // destroy entities with a Lifetime <= 0
                        if (!childrenFromEntity.IsEmpty)
                        {
                            List<Entity> entities = GetListOfEntities(entity, childrenFromEntity);
                            foreach(var item in entities)
                            {
                                EntityManager.DestroyEntity(item);
                            }
                        }
                    }
            }).Run();

    }

    // get a recursive list of child entities
    List<Entity> GetListOfEntities(Entity entity,  DynamicBuffer<Child> childrenFromEntity)
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


