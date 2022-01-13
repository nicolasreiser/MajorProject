using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int Currency;
    public int TopLevel;

    public int AbilityType;


    public SaveData( PlayerStats playerStats, int SaveType)
    {
        if(SaveType == 1)
        {
            // In Run Save
            this.Currency += playerStats.Currency;
            if (playerStats.TopLevel > this.TopLevel)
            {
                this.TopLevel = playerStats.TopLevel;
            }
        }
        if(SaveType == 0)
        {
            this.Currency = playerStats.Currency;
            if (playerStats.TopLevel > this.TopLevel)
            {
                this.TopLevel = playerStats.TopLevel;
            }
            this.AbilityType = playerStats.AbilityType;

        }
    }
}
