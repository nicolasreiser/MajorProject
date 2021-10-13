using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class PathFollowSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        GameObject obj = GameObject.Find("GridTest");
        Debug.Log("GridTestobj : " + obj);
        GridSetup gridTesting = GridSetup.Instance;
        Debug.Log("GridTesting : " + gridTesting);
        float2 origin = new float2(gridTesting.Origin.x,gridTesting.Origin.z);
        float cellSize = gridTesting.CellSize;

        Entities.ForEach((DynamicBuffer<PathPosition> pathPositionBuffer, ref Translation translation, ref PathFollow pathFollow) => 
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
                float movespeed = 3f;

                translation.Value += moveDir * movespeed * Time.DeltaTime;
                //Debug.Log("Target Position : " + targetPosition + "Path position : "+ pathPosition + " targetNode : " + targetNode);
                
                if(math.distance(translation.Value, targetPosition) < 0.1f)
                {
                    Debug.Log("Going to next point");
                    // go to next waypoint
                    pathFollow.pathIndex--;
                }
            }
        });
    }
}
