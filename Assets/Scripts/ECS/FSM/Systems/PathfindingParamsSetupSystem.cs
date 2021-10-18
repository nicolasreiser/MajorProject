using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;



//steps
// get player position - 
// get entities in pathfinding mode - 
// add and set the pathfindingparams - 
//
public class PathfindingParamsSetupSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecb;

    private EntityQuery EnemyQuery;

    private int2 playerGridPosition;
    GridSetup grid;

    protected override void OnCreate()
    {
        base.OnCreate();

        ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        EnemyQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<EnemyTag>() }
        });
    }
    protected override void OnUpdate()
    {
        grid = GridSetup.Instance;
        float deltaTime = Time.DeltaTime;
        // player position
        Entities.WithoutBurst().WithAll<PlayerTag>()
            .ForEach((Translation translate) =>
            {

                int2 newplayerGridPosition = grid.GetGridPosition(translate.Value);

                if (!newplayerGridPosition.Equals(playerGridPosition))
                {
                    playerGridPosition = newplayerGridPosition;
                    
                }
                
            }).Run();

        // Entities in Pathfinding mode

        var commandBuffer = ecb.CreateCommandBuffer();
        var ecbConcurrent = commandBuffer;
        
            Debug.Log("PLayer Grid Position : " + playerGridPosition);
            Entities.WithoutBurst().WithStoreEntityQueryInField(ref EnemyQuery)
                .ForEach((Entity entity,
                int entityInQueryIndex,
                 Translation translate,
                 ref PathfindState pathfindState) =>
                {
                     if(!pathfindState.targetPosition.Equals(playerGridPosition) && pathfindState.PathfindCooldown <= 0)
                    {
                        pathfindState.PathfindCooldown = 1f;
                        pathfindState.targetPosition = playerGridPosition;
                        Debug.Log("updates targetpos" + pathfindState.targetPosition);
                        int2 enemyGridPosition = grid.GetGridPosition(translate.Value);

                        ecbConcurrent.AddComponent<PathfindingParams>(entity);
                        ecbConcurrent.SetComponent(entity, new PathfindingParams
                        {
                            startPosition = enemyGridPosition,
                            endPosition = playerGridPosition
                        });
                    }
                    pathfindState.PathfindCooldown -= deltaTime;


                }).Run();

    }
}
