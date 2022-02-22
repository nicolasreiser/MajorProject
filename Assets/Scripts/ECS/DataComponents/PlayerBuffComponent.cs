using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// entity storing buff stats from the permanent upgrades

[GenerateAuthoringComponent]
public struct PlayerBuffComponent : IComponentData
{
    public int HealthBuff;
    public int DamageBuff;
    public int AttackspeedBuff;
    public int EarningsBuff;
}
