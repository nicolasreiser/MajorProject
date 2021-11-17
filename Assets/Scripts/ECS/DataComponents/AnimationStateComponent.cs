using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct AnimationStateComponent : IComponentData
{
    public AnimState AnimationState;
    public float TurnSpeed;
}

public enum AnimState
{
    Shoot,
    Run
}
