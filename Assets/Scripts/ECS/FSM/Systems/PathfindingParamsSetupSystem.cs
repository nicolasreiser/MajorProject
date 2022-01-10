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
        grid = GridSetup.Instance;

        if(grid == null)
        {
            Debug.LogWarning("The Grid has not been setup in the PathfindingParamSetup");

            return;
        }
        float deltaTime = Time.DeltaTime;
        // player position
        Entities.WithoutBurst().WithAll<PlayerTag>()
            .ForEach((in Translation translate) =>
            {

                int2 newplayerGridPosition = grid.GetGridPosition(translate.Value);

                if (!newplayerGridPosition.Equals(playerGridPosition))
                {
                    playerGridPosition = newplayerGridPosition;
                    
                }
                
            }).Run();

        // Entities in Pathfinding mode
        EnemyQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<EnemyTag>() }
        });

        var commandBuffer = ecb.CreateCommandBuffer();
        var ecbConcurrent = commandBuffer;
        
            Entities.WithoutBurst().WithStoreEntityQueryInField(ref EnemyQuery)
                .ForEach((Entity entity,
                int entityInQueryIndex,
                 ref Translation translate,
                 ref PathfindState pathfindState) =>
                {
                     if(!pathfindState.targetPosition.Equals(playerGridPosition) && pathfindState.PathfindCooldown <= 0)
                    {
                        pathfindState.PathfindCooldown = 1f;
                        pathfindState.targetPosition = playerGridPosition;
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
