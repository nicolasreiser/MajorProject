using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// Entity used to destroy a specific entity along with them
[GenerateAuthoringComponent]
public struct EntitiesToDestroyComponent : IComponentData
{
    public Entity entityToDestroy;
}
