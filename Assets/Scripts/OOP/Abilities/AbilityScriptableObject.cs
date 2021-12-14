using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Abilities")]

public class AbilityScriptableObject : ScriptableObject
{
    public string AbilityName;
    public float Cooldown;
    public string Description;
    public Sprite Picture;
    public Sprite InventoryIcon;
    public bool Unlocked;
    public bool Selected;
}
