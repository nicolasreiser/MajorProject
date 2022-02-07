using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int Currency;
    public int TopLevel;

    public int AbilityType;

    public List<bool> AbilitiesUnlocked;

    public int HealthBuff;
    public int DamageBuff;
    public int AttackspeedBuff;
    public int EarningsBuff;


    public SaveData( PlayerStats playerStats)
    {

        this.Currency = playerStats.RunCurrency + playerStats.TotalCurrency;
        if (playerStats.LastLevel > playerStats.TopLevel)
        {
            this.TopLevel = playerStats.LastLevel;
        }
        this.AbilityType = playerStats.AbilityType;

        AbilitiesUnlocked = new List<bool>();
        foreach(bool ability in playerStats.AbilitiesLock)
        {
            AbilitiesUnlocked.Add(ability);
        }

         this.HealthBuff = playerStats.HealthBuff;
        this.DamageBuff = playerStats.DamageBuff;
        this.AttackspeedBuff = playerStats.AttackspeedBuff;
        this.EarningsBuff = playerStats.EarningsBuff;
}
}
