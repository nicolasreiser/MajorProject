using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct EnemyDataContainer : IComponentData
{
    public int meleeHealth;
    public int meleeRange;
    public int meleeDamage;

    public int RangedHealth;
    public int RangedRange;
    public int RangedDamage;

    public int BombHealth;
    public int BombRange;
    public int BombDamage;
}
