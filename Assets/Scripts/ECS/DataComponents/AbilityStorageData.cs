using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct AbilityStorageData : IBufferElementData
{
    //public NativeString64 Name;
    public float Cooldown;
    public bool Unlocked;
    public bool Selected;
    public bool IsCast;

    public AbilityStorageData( float Cooldown, bool Unlocked, bool Selected)
    {
        //this.Name = Name;
        this.Cooldown = Cooldown;
        this.Unlocked = Unlocked;
        this.Selected = Selected;
        this.IsCast = false;
    }

}
