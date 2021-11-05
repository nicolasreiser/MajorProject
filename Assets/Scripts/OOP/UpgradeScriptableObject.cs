using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade")]

public class UpgradeScriptableObject : ScriptableObject
{
    public string Name;
    public string Description;
    public UpgradeType UpgradeType;
    public int PrimaryValue;
    public int SecondaryValue;

}

public enum UpgradeType
{
    DoubleShot,
    ParallelShot,
    ScatterShot,
    DamageMultiplier,
    AttackSpeed
}
