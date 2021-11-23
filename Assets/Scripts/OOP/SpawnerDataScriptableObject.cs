using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawner", menuName = "ScriptableObjects/Spawner")]

public class SpawnerDataScriptableObject : ScriptableObject
{
    public SpawnerData[] spawnerData;

}

[System.Serializable]
public struct SpawnerData
{
    public int EnemiesAmmount;
    public int InitialDelay;
    public int DelayBetweenSpawns;
    public EnemyType[] enemytypes;
}
