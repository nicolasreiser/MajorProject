using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Abilities/Nova")]

public class NovaScriptableObject : AbilityScriptableObject
{
    public float InternalCooldown;
    public float Duration;
}
