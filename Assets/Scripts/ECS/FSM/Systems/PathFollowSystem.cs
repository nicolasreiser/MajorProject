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
        GridTesting gridTesting = GridTesting.Instance;

        Entities.ForEach((DynamicBuffer<PathPosition> pathPositionBuffer, ref Translation translation, ref PathFollow pathFollow) => 
        {
            if(pathFollow.pathIndex >= 0)
            {
                
                int2 pathPosition = pathPositionBuffer[pathFollow.pathIndex].position;

                //simple movement

                float3 targetPosition = new float3(gridTesting.Origin.x +  pathPosition.x * gridTesting.CellSize,
                    translation.Value.y,
                    gridTesting.Origin.z +  pathPosition.y * gridTesting.CellSize );
                float3 moveDir = math.normalizesafe(targetPosition - translation.Value);
                float movespeed = 3f;

                Debug.Log("Enemy position : " + translation.Value + " Target : " + pathPosition + " Distance to goal : " + math.distance(translation.Value, targetPosition));
                translation.Value += moveDir * movespeed * Time.DeltaTime;

                
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
