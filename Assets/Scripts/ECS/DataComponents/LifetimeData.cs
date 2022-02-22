using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

//entity storing the remaining time left of entities

[GenerateAuthoringComponent]
public struct LifetimeData : IComponentData
{
    public float Lifetime;
    public bool ShouldDie;
}
