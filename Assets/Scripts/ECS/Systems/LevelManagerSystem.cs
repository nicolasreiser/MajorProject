using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine.SceneManagement;
using Unity.Transforms;

public class LevelManagerSystem : SystemBase
{
    private SceneStorage sceneStorage;
    private EnemiesSpawner enemiesSpawner;

    //private EntityManager entityManager;
    private Entity entityStorage;
    protected override void OnCreate()
    {


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
                    Debug.Log("Enemies spawner null : " + enemiesSpawner);
                    // TODO unload last level 
                    UnloadScene(levelDataComponent.currentLevel);

                    LoadScene(levelDataComponent.currentLevel+1);
                    ResetData(ref levelDataComponent);
                    //enemiesSpawner = Object.FindObjectOfType<EnemiesSpawner>();
                }

            }).Run();



        // Spawn player

        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
            {
                if(!levelDataComponent.PlayerSpawned)
                {
                    EntityQuery entityQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PrefabEntityStorage>());
                    entityStorage = entityQuery.GetSingletonEntity();

                    PrefabEntityStorage pes = EntityManager.GetComponentData<PrefabEntityStorage>(entityStorage);

                    var playerEntity = EntityManager.Instantiate(pes.Player);

                    // todo set spawn position
                    Translation t = EntityManager.GetComponentData<Translation>(playerEntity);

                    t.Value = new Vector3(2, 4, -7);
                    EntityManager.SetComponentData(playerEntity, t);
                    levelDataComponent.PlayerSpawned = true;
                }
            }).Run();

        // inject in spawner

        if (enemiesSpawner == null)
        {
            enemiesSpawner = Object.FindObjectOfType<EnemiesSpawner>();

        }

            if (enemiesSpawner != null)
            {
            Entities
                .WithoutBurst()
                .ForEach((Entity entity, ref LevelDataComponent levelDataComponent, in DynamicBuffer <SpawnerDataComponent> spawnerDataComponents) =>
                {
                    if (levelDataComponent.Inject)
                        return;

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

        

        // check for level completion

        if(enemiesSpawner != null)
        {
            //Debug.Log("Enemies spawner id : " + enemiesSpawner.GetInstanceID() + "Level completion : " + enemiesSpawner.EnemiesAmmount);
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
        }

        // give upgrades to player



        

        // reset Level data component
        
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
            {
                if (levelDataComponent.ReadyForReset)
                {
                    levelDataComponent.GetData = false;
                    levelDataComponent.GetScene = false;
                    levelDataComponent.Inject = false;
                    levelDataComponent.UpgradesReceived = false;
                    levelDataComponent.ReadyForReset = false;

                    levelDataComponent.ReadyForNextLevel = true;
                }

            }).Run();

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
