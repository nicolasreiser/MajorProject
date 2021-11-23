using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine.SceneManagement;


public class LevelManagerSystem : SystemBase
{
    private LevelDataComponent ldc;
    private SceneSystem sceneSystem;
    private SceneStorage sceneStorage;
    private SceneLevels currentScene;
    private EnemiesSpawner enemiesSpawner;

    protected override void OnCreate()
    {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
    }
    protected override void OnUpdate()
    {
        if(sceneStorage == null)
        {
            sceneStorage = SceneStorage.Instance;
        }
        // check for scene

        Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
            {
                if (levelDataComponent.ReadyForNextLevel)
                {
                    enemiesSpawner = null;
                    // TODO unload last level 
                    UnloadScene(levelDataComponent.currentLevel);

                    LoadScene(levelDataComponent.currentLevel+1);
                    ResetData(ref levelDataComponent);
                    enemiesSpawner = Object.FindObjectOfType<EnemiesSpawner>();
                }

            }).Run();



        // get data

        // inject in spawner

        if (enemiesSpawner == null)
        {
            enemiesSpawner = Object.FindObjectOfType<EnemiesSpawner>();

            if (enemiesSpawner)
            {
            Entities
                .WithoutBurst()
                .ForEach((Entity entity, ref LevelDataComponent levelDataComponent, in DynamicBuffer <SpawnerDataComponent> spawnerDataComponents) =>
                {
                    enemiesSpawner.EnemiesAmmount = spawnerDataComponents[levelDataComponent.currentLevel-1].EnemiesAmmount;
                    enemiesSpawner.InitialDelay = spawnerDataComponents[levelDataComponent.currentLevel - 1].InitialDelay;
                    enemiesSpawner.DelayBetweenSpawns = spawnerDataComponents[levelDataComponent.currentLevel - 1].DelayBetweenSpawns;

                    List<EnemyType> enemyTypes = new List<EnemyType>();
                    if(spawnerDataComponents[levelDataComponent.currentLevel - 1].MeleeEnemy)
                    {
                        enemyTypes.Add(EnemyType.Melee);
                    }
                    if (spawnerDataComponents[levelDataComponent.currentLevel - 1].RangedEnemy)
                    {
                        enemyTypes.Add(EnemyType.Ranged);
                    }
                    if (spawnerDataComponents[levelDataComponent.currentLevel - 1].BombEnemy)
                    {
                        enemyTypes.Add(EnemyType.Bomb);
                    }

                    EnemyType[] enemyTypesArray = enemyTypes.ToArray();
                    enemiesSpawner.EnemyTypes = enemyTypesArray;

                    levelDataComponent.Inject = true;
                }).Run();
            }

        }

        // check for level completion


        if(enemiesSpawner.CheckForLevelCleared())
        {
            Debug.Log("Level completed");

            Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
            {
                levelDataComponent.LevelCleared = true;
            }).Run();

        }

        // give upgrades to player

        // unload subscene

        // load new Subscene

        // reset Level data component


    }

    private void LoadScene(int level)
    {
           sceneStorage.LoadScene(level);

    }
    private void UnloadScene(int level)
    {
        sceneStorage.UnloadScene(level);
    }

    private void ResetData(ref LevelDataComponent ldc)
    {
        ldc.currentLevel++;
        ldc.GetData = false;
        ldc.GetScene = false;
        ldc. Inject = false;
        ldc. LevelCleared = false;
        ldc.UpgradesToGet = 0;
        ldc. UpgradesReceived = false;
        ldc. ReadyForNextLevel = false;
}

}
