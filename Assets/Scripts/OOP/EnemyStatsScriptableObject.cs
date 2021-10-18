using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTypes", menuName ="ScriptableObjects/Enemies")]
public class EnemyStatsScriptableObject : ScriptableObject  
{
    EnemyType enemyType;
    public int health;
    public int Range;
    public int dammage;

}
