using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


public struct EnemyDataContainer : IBufferElementData
{
    public int Health;
    public int Range;
    public int Damage;
    public int MaxRange;
    public int DetectionRange;
    public float Cooldown;
    public int Experience;

    public EnemyDataContainer(int Health, int Range, int Damage, int MaxRange, int DetectionRange, float Cooldown, int Experience)
    {
        this.Health = Health;
        this.Range = Range;
        this.Damage = Damage;
        this.MaxRange = MaxRange;
        this.DetectionRange = DetectionRange;
        this.Cooldown = Cooldown;
        this.Experience = Experience;
    }

}
