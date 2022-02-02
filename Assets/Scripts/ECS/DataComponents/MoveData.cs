using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct MoveData : IComponentData
{
    public float3 direction;
    public float3 lastDirection;
    public float speed;
    public float turnSpeed;
    public float maxVelocity;
}
