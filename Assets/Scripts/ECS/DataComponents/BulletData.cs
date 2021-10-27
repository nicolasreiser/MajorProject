using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct BulletData : IComponentData
{
    public BulletOrigin Origin;
    public int Damage;
    public bool ScatterShot;
    public int ScatterShotDamage;
    public float ScatterShotCooldown;
    public bool ShouldDestroy;
}

public enum BulletOrigin
{
    Player,
    Enemy
}
