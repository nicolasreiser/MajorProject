using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public class HealthBarData : IComponentData
{
    public Slider slider;
    public Canvas canvas;
    public Camera camera;
    public float3 Offset;

}
