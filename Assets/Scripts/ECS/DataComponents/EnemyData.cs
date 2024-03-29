using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// entity containing the enemy stats
[GenerateAuthoringComponent]
public struct EnemyData : IComponentData
{
    public int BaseHealth;
    public int CurrentHealth;
    public int Experience;
    public int Gold;
    public bool hasGivenGold;
    public bool hasGivenExperience;
    public bool hasCastEffects;
    public bool ScatterShot;
    public int ScatterShotDamage;
    public float ScatterShotCooldown;

}
