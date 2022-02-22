using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// entity storing spawner data

[GenerateAuthoringComponent]
public struct SpawnerTriggerComponent : IComponentData
{
    public bool isActive;
}
