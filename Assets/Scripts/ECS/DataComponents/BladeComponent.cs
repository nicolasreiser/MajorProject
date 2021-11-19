using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct BladeComponent : IComponentData
{
    public Entity Blade;
    public int ScaleSpeed;
    public int SpinSpeed;
}
