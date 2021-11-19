using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
struct EnemyTypeData : IComponentData
{
    public EnemyType enemyType;
}

public enum EnemyType
{
    Melee,
    Ranged,
    Bomb
}
