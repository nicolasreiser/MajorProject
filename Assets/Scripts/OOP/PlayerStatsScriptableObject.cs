using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/Player")]

public class PlayerStatsScriptableObject : ScriptableObject
{
    public int[] Health;
    public int[] Experience;
}
