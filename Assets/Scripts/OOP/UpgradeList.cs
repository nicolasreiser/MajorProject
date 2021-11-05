using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeList : MonoBehaviour
{
    [SerializeField] private UpgradeScriptableObject[] Upgrades; 

   
    public UpgradeScriptableObject getRandomUpgrade()
    {
        int res = Random.Range(0, Upgrades.Length);

        return Upgrades[res];
    }
}
