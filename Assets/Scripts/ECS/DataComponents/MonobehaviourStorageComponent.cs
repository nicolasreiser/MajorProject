using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public class MonobehaviourStorageComponent : IComponentData
{
    public Camera UICamera;
    public Canvas UICanvas;
}
