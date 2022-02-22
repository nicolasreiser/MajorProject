using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// Entity storing specific animation data
[GenerateAuthoringComponent]
public struct AnimationHolderComponent : IComponentData
{
    public Entity entity;
    public float TurnSpeed;
}
