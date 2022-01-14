using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public struct AbilityData : IComponentData
{
    public int AbilityType;
    public float BaseCooldown;
    public float CurrentCooldown;
    public float Duration;
    public bool IsCast;
}