using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using Unity.Mathematics;

// entity storing references to monobehaviours needed in systems

[GenerateAuthoringComponent]
public class HealthBarData : ISystemStateComponentData
{
    public Slider slider;
    public Canvas canvas;
    public Camera camera;
    public float3 Offset;

}
