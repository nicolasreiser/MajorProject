using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
[GenerateAuthoringComponent]
public struct LifetimeData : IComponentData
{
    public float Lifetime;
    public bool ShouldDie;
}
