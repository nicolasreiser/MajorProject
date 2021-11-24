using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct LevelDataComponent : IComponentData
{
    public bool GetScene;
    public int currentLevel;
    public bool GetData;
    public bool PlayerSpawned;
    public bool Inject;
    public bool LevelCleared;
    public int UpgradesToGet;
    public bool UpgradesReceived;
    public bool ReadyForReset;
    public bool ReadyForNextLevel;
}
