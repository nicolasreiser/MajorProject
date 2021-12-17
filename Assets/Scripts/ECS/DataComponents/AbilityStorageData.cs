using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

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
    }

}

public enum AbilityType
{
    BigBadBuff = 1,
    Dash = 2,
    Nova = 3
}
