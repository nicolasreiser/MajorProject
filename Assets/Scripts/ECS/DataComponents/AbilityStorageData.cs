using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

// Container storing the data of every ability usable 
public struct AbilityStorageData : IBufferElementData
{
    public int Ability;
    public float Cooldown;
    public bool Unlocked;
    public bool Selected;
    public bool IsCast;

    public float DamageModifier;
    public float AttackspeedModifier;
    public float Duration;
    public float IntervalCooldown;
    public AbilityStorageData( AbilityType abilityType, float Cooldown, bool Unlocked, bool Selected)
    {
        Ability = (int)abilityType;
        this.Cooldown = Cooldown;
        this.Unlocked = Unlocked;
        this.Selected = Selected;
        this.IsCast = false;
        DamageModifier = 1;
        AttackspeedModifier = 1;
        Duration = 0;
        this.IntervalCooldown = 0;
    }

    public AbilityStorageData(AbilityType abilityType, float Cooldown, bool Unlocked, bool Selected, float DamageModifier, float AttackspeedModifier, float Duration)
    {
        Ability = (int)abilityType;
        this.Cooldown = Cooldown;
        this.Unlocked = Unlocked;
        this.Selected = Selected;
        this.IsCast = false;
        this.AttackspeedModifier = AttackspeedModifier;
        this.DamageModifier = DamageModifier;
        this.Duration = Duration;
        this.IntervalCooldown = 0;

    }

    public AbilityStorageData(AbilityType abilityType, float Cooldown, bool Unlocked, bool Selected, float Duration, float InternalCooldown)
    {
        Ability = (int)abilityType;
        this.Cooldown = Cooldown;
        this.Unlocked = Unlocked;
        this.Selected = Selected;
        this.IsCast = false;
        DamageModifier = 1;
        AttackspeedModifier = 1;
        this.Duration = Duration;
        this.IntervalCooldown = InternalCooldown;
    }

    public AbilityStorageData(AbilityType abilityType, float Cooldown, bool Unlocked, bool Selected, float Duration)
    {
        Ability = (int)abilityType;
        this.Cooldown = Cooldown;
        this.Unlocked = Unlocked;
        this.Selected = Selected;
        this.IsCast = false;
        DamageModifier = 1;
        AttackspeedModifier = 1;
        this.Duration = Duration;
        this.IntervalCooldown = 0;
    }
}

public enum AbilityType
{
    None = 0,
    BigBadBuff = 1,
    Dash = 2,
    Nova = 3
}
