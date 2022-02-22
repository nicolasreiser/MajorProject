using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// Entity storing data for the melee enemy's blade animation
[GenerateAuthoringComponent]
public struct BladeComponent : IComponentData
{
    public Entity Blade;
    public int ScaleSpeed;
    public int SpinSpeed;
}
