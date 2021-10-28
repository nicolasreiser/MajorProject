using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct EnemyData : IComponentData
{
    public int BaseHealth;
    public int CurrentHealth;
    public bool ScatterShot;
    public int ScatterShotDamage;
    public float ScatterShotCooldown;

}
