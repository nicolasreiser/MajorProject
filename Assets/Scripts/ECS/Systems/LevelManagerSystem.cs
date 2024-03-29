using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine.SceneManagement;
using Unity.Transforms;

// system controlling the loop of a level and corrept initialisation and execution
public class LevelManagerSystem : SystemBase
{
    private SceneStorage sceneStorage;
    private EnemiesSpawner enemiesSpawner;
    private MonobehaviourStorageComponent monobehaviourStorageComponent;

    private Entity entityStorage;
    private int loadedLevel;
    

    protected override void OnCreate()
    {
        //loadedLevel = 0;
        base.OnCreate();
    }
    protected override void OnUpdate()
    {
        //TODO is in game check


        PauseManagement pm = PauseManagement.Instance;

        if(pm != null)
        {
            if (pm.IsPaused)
            {
                return;
            }
        }

        float deltatime = Time.DeltaTime;
        if(sceneStorage == null)
        {
            sceneStorage = SceneStorage.Instance;
        }

        // initialisation
        Entities
         .WithoutBurst()
         .WithNone<PausedTag>()
         .ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
         {
             if (!levelDataComponent.Initialisation)
             {
                 Debug.Log("Initialising LDC System");
                 loadedLevel = 0;
                 levelDataComponent.Initialisation = true;

             }
         }).Run();

        // check and load/ unload scene

        Entities
            .WithoutBurst()
            .WithNone<PausedTag>()
            .ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
            {
                if (levelDataComponent.ReadyForNextLevel && levelDataComponent.CleanupObstacles && levelDataComponent.hasSaved && levelDataComponent.hasLoadedSave)
                {
                    enemiesSpawner = null;

                    ResetData(ref levelDataComponent);

                    if(levelDataComponent.isStartLevel)
                    {

                        levelDataComponent.isStartLevel = false;
                        sceneStorage.UnLoadStartLevel();
                        //Debug.Log("Unloaded Start Level");
                    }
                    else if(levelDataComponent.isEndLevel)
                    {
                        sceneStorage.UnLoadEndLevel();
                    }
                    else if(levelDataComponent.isMenu)
                    {
                       
                    }
                    else
                    {

                        sceneStorage.UnloadLevel(loadedLevel);
                    }

                    if (levelDataComponent.currentLevel == 1)
                    {

                        levelDataComponent.isStartLevel = true;
                        sceneStorage.LoadStartLevel();
                        //Debug.Log("Loaded Start Level");

                    }
                    else if(levelDataComponent.currentLevel == 5)
                    {
                        sceneStorage.LoadEndLevel();
                        levelDataComponent.isEndLevel = true;

                    }
                    else
                    {

                        loadedLevel = Random.Range(1,sceneStorage.SceneLength()+1);
                        for (int i = 0; i < 5; i++)
                        {
                            loadedLevel = Random.Range(1, sceneStorage.SceneLength() + 1);

                           // Debug.Log("Random Number " +i+" = " + loadedLevel);
                        }

                        //Debug.Log("Loading scene level " + loadedLevel + " Scenelist length : " + sceneStorage.SceneLength());
                        sceneStorage.LoadLevel(loadedLevel);
                    }
                    
                }

            }).Run();

        EntityQuery playerSpawnPositionQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerSpawnPositionTag>(), ComponentType.ReadOnly<Translation>());

        Translation playerSpawnPosition = new Translation();

        // gerplayer spawn position
        
        if (!playerSpawnPositionQuery.IsEmpty)
        {
            playerSpawnPosition = EntityManager.GetComponentData<Translation>(playerSpawnPositionQuery.GetSingletonEntity());

        }

       // spawn player

         Entities.WithoutBurst().
        ForEach((Entity entity, MonobehaviourStorageComponent storage) =>
        {
            monobehaviourStorageComponent = storage;
        }).Run();
       
         if (monobehaviourStorageComponent.MainCanvas != null)
       
         {
             Entities
                 .WithoutBurst()
                 .WithStructuralChanges()
                 .WithNone<PausedTag>()
                 .ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
                 {
                     
                     if (!levelDataComponent.PlayerSpawned && !playerSpawnPositionQuery.IsEmpty && levelDataComponent.PlayerSpawnTimer <= 0)
                     {
                         EntityQuery entityQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PrefabEntityStorage>());
                         entityStorage = entityQuery.GetSingletonEntity();
       
                         PrefabEntityStorage pes = EntityManager.GetComponentData<PrefabEntityStorage>(entityStorage);
       
                         var playerEntity = EntityManager.Instantiate(pes.Player);
       
                         levelDataComponent.ActivePlayer = true;
                         levelDataComponent.PlayerSpawned = true;
       
                         //Debug.Log("Player Spawned 1");
                     }
                     if (!levelDataComponent.PlayerSetPosition && levelDataComponent.PlayerSpawnTimer <= 0)
                     {
                         //Debug.Log("1");
                         levelDataComponent.ActivePlayer = true;
       
                         EntityQuery entityQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerData>(), ComponentType.ReadWrite<Translation>());
                         if (entityQuery.IsEmpty)
                             return;
       
                         //Debug.Log("2");
       
                         Translation t = EntityManager.GetComponentData<Translation>(entityQuery.GetSingletonEntity());
                         Vector3 pos = playerSpawnPosition.Value;
                         t.Value = pos;
                         EntityManager.SetComponentData(entityQuery.GetSingletonEntity(), t);
                         levelDataComponent.PlayerSetPosition = true;
       
                         CanvasPanelManagement cpm = monobehaviourStorageComponent.MainCanvas.GetComponent<CanvasPanelManagement>();
                         cpm.PanelState(false,0);
       
                         Debug.Log("Player Spawned 2");
       
                     }
                     levelDataComponent.PlayerSpawnTimer -= deltatime;
                 }).Run();
         }
        

        // inject in spawner

        //if (enemiesSpawner == null)
        //{
        //    enemiesSpawner = Object.FindObjectOfType<EnemiesSpawner>();
        //}
        //if (enemiesSpawner != null)
        {
        Entities
            .WithoutBurst()
            .WithNone<PausedTag>()
            .ForEach((Entity entity, ref LevelDataComponent levelDataComponent, in DynamicBuffer <SpawnerDataComponent> spawnerDataComponents) =>
            {
                if (levelDataComponent.Inject )
                    return;
                
                if(levelDataComponent.SpawnerTimer > 0)
                {
                    levelDataComponent.SpawnerTimer-= deltatime;
                    return;
                }
                
                enemiesSpawner = Object.FindObjectOfType<EnemiesSpawner>();

                Debug.Log("Spawner Injected with ID " + enemiesSpawner.GetInstanceID());
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
            if(enemiesSpawner.CheckForLevelCleared())
            {
                Entities
                .WithoutBurst()
                .WithNone<PausedTag>()
                .ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
                {
                    levelDataComponent.LevelCleared = true;
                    levelDataComponent.PlayerInvulnerability = true;
                }).Run();
            }
        }

        // give upgrades to player

        
        if(monobehaviourStorageComponent!= null)
        {
            Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
            {
                if (levelDataComponent.LevelCleared && !levelDataComponent.CompletionUI)
                {
                    CanvasUpgrades cu = monobehaviourStorageComponent.MainCanvas.GetComponent<CanvasUpgrades>();
                    if(!levelDataComponent.isEndLevel)
                    {
                        cu.LevelCompleted();
                    }
                    else
                    {
                        cu.RunCompleted();
                        levelDataComponent.ReadyForReset = true;
                    }
                    levelDataComponent.CompletionUI = true;
                }
            }).Run();
        }

        //Exit Trigger

        EntityQuery query = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<ExitTriggerComponent>());
        ExitTriggerComponent etc = new ExitTriggerComponent();

        if (!query.IsEmpty)
            etc = EntityManager.GetComponentData<ExitTriggerComponent>(query.GetSingletonEntity());
        
        
        // Give Upgrades

        if (monobehaviourStorageComponent != null)
        {
            Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
            {
                if (!levelDataComponent.UpgradesReceived && levelDataComponent.UpgradesToGet > 0 && !levelDataComponent.Upgrading && levelDataComponent.LevelCleared && !levelDataComponent.isEndLevel)
                {
                    if (levelDataComponent.UpgradesTimer >= 0)
                    {
                        levelDataComponent.UpgradesTimer -= deltatime;
                        return;
                    }
                    CanvasUpgrades cu = monobehaviourStorageComponent.MainCanvas.GetComponent<CanvasUpgrades>();

                    cu.ToggleUI();

                    levelDataComponent.Upgrading = true;
                    etc.Exit = false;
                    EntityManager.SetComponentData(query.GetSingletonEntity(), etc);
                }


            }).Run();
        }

        // reset Level data component

        Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
            {
            if (levelDataComponent.ReadyForReset && etc.Exit)
                {
                    if (levelDataComponent.isEndLevel)
                    {
                        CanvasPanelManagement cpm = monobehaviourStorageComponent.MainCanvas.GetComponent<CanvasPanelManagement>();
                        cpm.LoadMainMenu();
                    }

                    if(!levelDataComponent.TransitionPanel)
                    {
                        CanvasPanelManagement cpm = monobehaviourStorageComponent.MainCanvas.GetComponent<CanvasPanelManagement>();
                        cpm.PanelState(true,0);
                    }

                    if (levelDataComponent.ExitTimer <= 0)
                    {
                        levelDataComponent.ActivePlayer = false;
                        levelDataComponent.ReadyForNextLevel = true;
                        levelDataComponent.CleanupObstacles = false;
                        levelDataComponent.hasSaved = false;
                    }
                    levelDataComponent.ExitTimer -= deltatime;
                }
            }).Run();

    }

    

    private void ResetData(ref LevelDataComponent ldc)
    {
        ldc.currentLevel++;
        ldc.GetData = false;
        ldc.GetScene = false;
        ldc.Inject = false;
        ldc.LevelCleared = false;
        ldc.UpgradesToGet = 0;
        ldc.UpgradesReceived = false;
        ldc.ReadyForNextLevel = false;
        ldc.CompletionUI = false;
        ldc.PlayerSetPosition = false;
        ldc.ReadyForReset = false;
        ldc.PlayerInvulnerability = false;
        ldc.ActivePlayer = false;
        ldc.TransitionPanel = false;
        //ldc.CleanupObstacles = false;


        ldc.PlayerSpawnTimer = 2;
        ldc.UpgradesTimer = 2;
        ldc.ExitTimer = 2;
        ldc.SpawnerTimer = 2;

        Debug.Log("Data Reset");
    }

}
