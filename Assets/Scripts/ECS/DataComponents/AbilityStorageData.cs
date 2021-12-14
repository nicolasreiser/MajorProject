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

    public AbilityStorageData( AbilityType abilityType, float Cooldown, bool Unlocked, bool Selected)
    {
        Ability = (int)abilityType;
        this.Cooldown = Cooldown;
        this.Unlocked = Unlocked;
        this.Selected = Selected;
        this.IsCast = false;
    }

}

public enum AbilityType
{
    BigBadBuff = 1,
    Dash = 2,
    Nova = 3
}
