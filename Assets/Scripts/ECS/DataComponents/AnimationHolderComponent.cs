using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
[GenerateAuthoringComponent]
public struct AnimationHolderComponent : IComponentData
{
    public Entity entity;
    public float TurnSpeed;
}
