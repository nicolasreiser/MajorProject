using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct EntitiesToDestroyComponent : IComponentData
{
    public Entity entityToDestroy;
}
