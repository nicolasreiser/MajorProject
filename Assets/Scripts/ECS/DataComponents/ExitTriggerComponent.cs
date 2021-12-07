using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct ExitTriggerComponent : IComponentData
{
    public bool Exit;
}
