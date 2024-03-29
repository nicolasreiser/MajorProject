using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// system iterating over entities pathfinding to controll their movement
public class PathFollowSystem : ComponentSystem
{
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

        GridSetup gridTesting = GridSetup.Instance;

        if(gridTesting == null)
        {
            Debug.LogWarning("The Grid has not been setup in the PathFollowSystem");
            return;
        }
        float2 origin = new float2(gridTesting.Origin.x,gridTesting.Origin.z);
        float cellSize = gridTesting.CellSize;

        Entities.ForEach((DynamicBuffer<PathPosition> pathPositionBuffer, ref Translation translation, ref PathFollow pathFollow, ref MoveData moveData) => 
        {
            if(pathFollow.pathIndex >= 0)
            {
                
                int2 pathPosition = pathPositionBuffer[pathFollow.pathIndex].position;

                //simple movement


                float3 targetPosition = new float3(origin.x +  pathPosition.x * cellSize + cellSize/2,
                    translation.Value.y,
                    origin.y +  pathPosition.y * cellSize + cellSize / 2);
                int2 targetNode = new int2(pathPosition.x, pathPosition.y);

                float3 moveDir = math.normalizesafe(targetPosition - translation.Value );
                
                translation.Value += moveDir * moveData.speed * Time.DeltaTime;

                moveData.direction = moveDir;

                if(math.distance(translation.Value, targetPosition) < 0.1f)
                {
                    // go to next waypoint
                    pathFollow.pathIndex--;
                }
            }
        });
    }
}
