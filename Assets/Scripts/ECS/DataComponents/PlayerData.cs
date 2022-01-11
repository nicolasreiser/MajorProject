using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerData : IComponentData
{
    public int Level;
    public int BaseHealth;
    public int CurrentHealth;
    public int Experience;
    public int MaxExperience;
    public int OverflowExperience;

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

    public float AttackSpeedModifier;
    public float AttackDamageModifier;

    public int BulletDamagePercentage;

    public float StandStillTimer;
    public float PlayerRange;
    public bool IsInvulnerable;
    public bool OnHealthChange;
    public bool OnExperienceChange;
    public bool Initialised;

    public bool IsDead;

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
