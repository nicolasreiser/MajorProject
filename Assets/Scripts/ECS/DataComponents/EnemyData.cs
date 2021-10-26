using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct EnemyData : IComponentData
{
    public int Health;
    public bool ScatterShot;
    public int ScatterShotDamage;

}
