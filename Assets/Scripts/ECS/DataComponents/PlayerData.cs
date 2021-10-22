using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerData : IComponentData
{
    public int Health;
    public int gold;

    public int WeaponBaseDamage;
    public float WeaponCooldown;
    public float WeaponBaseCooldown;
    public int DoubleShot;
    public int ParallelShot;
    public int PiercingShot;
    public int ScatterShot;
    public int DamageAmplification;

    public int BulletDamagePercentage;

    public float StandStillTimer;
    public float PlayerRange;
}
