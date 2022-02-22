using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// Entity defining an enemy's type
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
