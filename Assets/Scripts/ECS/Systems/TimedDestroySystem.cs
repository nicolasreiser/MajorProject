using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

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
        float deltaTime = Time.DeltaTime;

        var commandBuffer = ecb.CreateCommandBuffer().AsParallelWriter();

        Entities.WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, int entityInQueryIndex, ref LifetimeData lifetimeData, ref DynamicBuffer<Child> childrenFromEntity) =>
            {
                Debug.Log("Entering death1 children : "+ childrenFromEntity.Length);

            if (lifetimeData.ShouldDie)
            {
                    if(!childrenFromEntity.IsEmpty)
                    {
                        List<Entity> entities = GetListOfEntities(entity, childrenFromEntity);
                        Debug.Log("List of entities : " + entities.Count);
                        foreach (var item in entities)
                        {
                            EntityManager.DestroyEntity(item);
                        }
                    }

                return;
            }
            lifetimeData.Lifetime -= deltaTime;
            if (lifetimeData.Lifetime <= 0)
            {
                    if (!childrenFromEntity.IsEmpty)
                    {
                        List<Entity> entities = GetListOfEntities(entity, childrenFromEntity);
                        Debug.Log("List of entities : " + entities.Count);
                        foreach(var item in entities)
                        {
                            EntityManager.DestroyEntity(item);
                        }
                    }

                }
            }).Run();

    }
    List<Entity> GetListOfEntities(Entity entity,  DynamicBuffer<Child> childrenFromEntity)
    {
        List<Entity> entities = new List<Entity>();
        BufferFromEntity<Child> childBuffer = this.GetBufferFromEntity<Child>(true);

        foreach (var child in childrenFromEntity)
        {
            var childEntity = child.Value;
            bool hasChild = childBuffer.HasComponent(childEntity);

            if (hasChild)
            {
                var childbuffer = EntityManager.GetBuffer<Child>(childEntity);
                List<Entity> childEntitiesList = GetListOfEntities(childEntity, childbuffer);
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
        return entities;
    }
}


