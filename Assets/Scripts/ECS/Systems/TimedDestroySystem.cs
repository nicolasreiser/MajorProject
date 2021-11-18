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

        //Entities.WithoutBurst()
        //    .WithStructuralChanges()
        //    .ForEach((Entity entity, ref LifetimeData lifetimeData, ref EntitiesToDestroyComponent entitiesList ) =>
        //    {
        //        Debug.Log("Entering death2 , next entity to destroy : " + entitiesList.entityToDestroy.Index );
        //        if (lifetimeData.ShouldDie)
        //        {

        //            EntityManager.DestroyEntity(entity);
        //            return;
        //        }
        //        lifetimeData.Lifetime -= deltaTime;
        //        if (lifetimeData.Lifetime <= 0)
        //        {
        //            Debug.Log("Destroying entity : " + entitiesList.entityToDestroy.Index);

        //            EntityManager.DestroyEntity(entitiesList.entityToDestroy);

        //            EntityManager.DestroyEntity(entity);
        //        }
        //    }).Run();

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
                        //DestroyHierarchy(entity,entityInQueryIndex,ref commandBuffer,childrenFromEntity);
                    }

                //EntityManager.DestroyEntity(entity);
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
                        //DestroyHierarchy(entity, entityInQueryIndex, ref commandBuffer, childrenFromEntity);
                    }

                }
            }).Run();

        //return inputDeps;
    }

    void DestroyHierarchy( Entity entity,int index, ref EntityCommandBuffer.ParallelWriter ecb,  DynamicBuffer<Child> childrenFromEntity)
    {
        Debug.Log("Entering DestroyHierarchy from entity : "+ entity.Index);
        BufferFromEntity<Child> childBuffer = this.GetBufferFromEntity<Child>(true);
        
        foreach (var child in childrenFromEntity)
        {
            var childEntity = child.Value;

            bool haschild = childBuffer.HasComponent(childEntity);

            Debug.Log("Entity : "+childEntity.Index+" HasChild : " + haschild);
            if(haschild)
            {
                var childbuffer = EntityManager.GetBuffer<Child>(childEntity);

                DestroyHierarchy(childEntity,childEntity.Index,ref ecb, childbuffer);
                 
            }
            else
            {
                Debug.Log("Destroyed entity : " + childEntity.Index);
                EntityManager.DestroyEntity(childEntity);
                //ecb.DestroyEntity(childEntity, childEntity.Index);

            }
        }
        Debug.Log("Destroyed entity : " + entity.Index);

        EntityManager.DestroyEntity(entity);

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
    //void DestroyHierarchy(EntityCommandBuffer.ParallelWriter cmdBuffer, Entity entity, int index, BufferFromEntity<Child> childrenFromEntity)
    //{
    //    if (!childrenFromEntity.HasComponent(entity))
    //        return;
    //    var children = childrenFromEntity[entity];
    //    for (var i = 0; i < children.Length; ++i)
    //    {
    //        var childEntity = children[i].Value;
    //        cmdBuffer.DestroyEntity(index, childEntity);
    //        DestroyHierarchy(cmdBuffer, childEntity, index, childrenFromEntity);
    //    }
    //}
}


