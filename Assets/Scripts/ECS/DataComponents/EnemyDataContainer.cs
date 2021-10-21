using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct EnemyDataContainer : IComponentData
{
    public int MeleeHealth;
    public int MeleeRange;
    public int MeleeDamage;
    public int MeleeMaxRange;
    public float MeleeCooldown;

    public int RangedHealth;
    public int RangedRange;
    public int RangedDamage;
    public int RangedMaxRange;
    public float RangedCooldown;


    public int BombHealth;
    public int BombRange;
    public int BombDamage;
    public int BombMaxRange;
    public float BombCooldown;

}