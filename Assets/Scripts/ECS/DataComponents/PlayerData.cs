using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerData : IComponentData
{
    public int Health;
    public int gold;

    public float StaticWeaponCooldown;
    public int WeaponBaseDamage;
    public float WeaponCooldown;
    public float DoubleShotCooldown;
    public float WeaponBaseCooldown;


    public int DoubleShot;
    public int ParallelShot;
    public int PiercingShot;
    public int ScatterShot;
    public int DamageAmplification;
    public float AttackSpeed;

    public int BulletDamagePercentage;

    public float StandStillTimer;
    public float PlayerRange;
    public bool isInvulnerable;

    public void EditDoubleSHot(int value)
    {
        DoubleShot += value;
    }
    public void EditParallelShot(int value)
    {
        ParallelShot += value;
    }
    public void EditPiercingShot(int value)
    {
        PiercingShot += value;
    }
    public void EditDamageAmplification(int value)
    {
        DamageAmplification += value;
        BulletDamagePercentage += DamageAmplification;
    }
    public void EditScatterShot(int value)
    {
        ScatterShot += value;
    }
    public void EditAttackSpeed(int value)
    {
        AttackSpeed += value;
        WeaponBaseCooldown = (StaticWeaponCooldown / ((AttackSpeed + 100)/ 100));
    }
    public void ResetValues()
    {
        DoubleShot = 0;
        ParallelShot = 0;
        PiercingShot = 0;
        ScatterShot = 0;
        DamageAmplification = 0;
        AttackSpeed = 0;
        BulletDamagePercentage = 100;
    }
}
