using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(50)]
public struct PathPosition : IBufferElementData
{
    public int2 position;
}
