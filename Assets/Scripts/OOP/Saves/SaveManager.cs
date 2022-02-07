using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    public static void SaveStats(PlayerStats playerStats)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/playerSave.txt";
        FileStream stream = new FileStream(path,FileMode.Create);

        SaveData data = new SaveData(playerStats);

        formatter.Serialize(stream, data);
        stream.Close();
    }
   
    public static SaveData LoadStats()
    {
        string path = Application.persistentDataPath + "/playerSave.txt";
        if(File.Exists(path))
        {
            BinaryFormatter formatter=new BinaryFormatter();
            FileStream stream = new FileStream(path,FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;

            stream.Close();
            return data;


        }
        else
        {
            Debug.LogError("Save file not found, creating base file");

            PlayerStats playerStats = GenerateDefaultSave();


            SaveStats(playerStats);

            


            return LoadStats();
        }
    }

    public static void DeleteSaves()
    {
        string path = Application.persistentDataPath + "/playerSave.txt";
        if (File.Exists(path))
        {
            File.Delete(path);

            Debug.Log("Saves Deleted");
        }
    }

    public static  PlayerStats GenerateDefaultSave()
    {
        PlayerStats playerStats = new PlayerStats();
        playerStats.AbilitiesLock = new List<bool>();
        playerStats.AbilitiesLock.Add(true);
        playerStats.AbilitiesLock.Add(false);
        playerStats.AbilitiesLock.Add(false);
        playerStats.AbilitiesLock.Add(false);

        playerStats.RunCurrency = 0;
        playerStats.TotalCurrency = 0;
        playerStats.TopLevel = 0;
        playerStats.LastLevel = 0;
        playerStats.AbilityType = 0;
        playerStats.HealthBuff = 0;
        playerStats.DamageBuff = 0;
        playerStats.AttackspeedBuff = 0;
        playerStats.EarningsBuff = 0;

        return playerStats;
    }

}
