using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct PathfindingParams : IComponentData
{
    public int2 startPosition;
    public int2 endPosition;

}
