using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// Entity storing which state the animation is in
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
