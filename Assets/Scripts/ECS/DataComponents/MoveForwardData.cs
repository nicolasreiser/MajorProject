using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

//entity attached to objects supposed to permanently move forward (bullets)

[GenerateAuthoringComponent]
public struct MoveForwardData : IComponentData
{
    public float velocity;
}
