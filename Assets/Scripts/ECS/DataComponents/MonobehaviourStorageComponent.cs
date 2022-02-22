using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// entity storing monobehaviour references

[GenerateAuthoringComponent]
public class MonobehaviourStorageComponent : IComponentData
{
    public Camera UICamera;
    public Canvas UICanvas;
    public Canvas MainCanvas;
}
