using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    public static void SaveStats(PlayerStats playerStats, int SaveType)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/plaverSave.txt";
        FileStream stream = new FileStream(path,FileMode.Create);

        SaveData data = new SaveData(playerStats, SaveType);

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
            Debug.LogError("Save file not found");
            return null;
        }
    }

}
