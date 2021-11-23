using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct IdleState : IComponentData
{
    public float PlayerDistance;
    public int EnemyDetectionRange;

}
