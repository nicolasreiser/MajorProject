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
    public bool PlayerSetPosition;
    public bool Inject;
    public bool LevelCleared;

    public int UpgradesToGet;
    public bool UpgradesReceived;
    public bool Upgrading;
    public bool ReadyForReset;
    public bool CompletionUI;
    public bool ReadyToExit;
    public bool ReadyForNextLevel;
    public bool PlayerInvulnerability;
    public bool ActivePlayer;
    public bool TransitionPanel;
    public bool CleanupObstacles;

    public float PlayerSpawnTimer;
    public float UpgradesTimer;
    public float ExitTimer;

    public bool isMenu;
    public bool isStartLLevel;
    public bool isEndLevel;

}
