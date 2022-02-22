using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// entity storing if a trigegr has been affected
[GenerateAuthoringComponent]
public struct ExitTriggerComponent : IComponentData
{
    public bool Exit;
}
