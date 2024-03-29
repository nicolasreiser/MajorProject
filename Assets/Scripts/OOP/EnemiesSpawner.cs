using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

// manages enemy spawning 
public class EnemiesSpawner : MonoBehaviour
{

    public Vector2 SpawnerSize;
    public int EnemiesAmmount;
    public EnemyType[] EnemyTypes;
    public float InitialDelay;
    public float DelayBetweenSpawns;
    private float currentDelayBetweenSpawns;

    private EntityManager entityManager;
    private SpawnerTriggerComponent spawnerTrigger;

    private bool IsActive = false;

    private Entity entityStorage;

    private PauseManagement pm;

    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        currentDelayBetweenSpawns = 0;
        GetPrefabs();
        pm = PauseManagement.Instance;

    }

    // Update is called once per frame
    void Update()
    {
        if(pm != null)
        {
            if(pm.IsPaused)
            {
                return;
            }
        }
        GetTrigger();
        if (!IsActive)
            return;

        InitialDelay -= Time.deltaTime;
        
        if(InitialDelay <= 0)
        {
            if(currentDelayBetweenSpawns <= 0 && EnemiesAmmount > 0)
            {
                // spawn 1 enemy
                SpawnEntity(GetRandomEnemyType());
                EnemiesAmmount--;
                currentDelayBetweenSpawns = DelayBetweenSpawns;

            }
            currentDelayBetweenSpawns -= Time.deltaTime;
        }
    }

    // gets the component containing the entity prefabs
    private void GetPrefabs()
    {
        EntityQuery entityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PrefabEntityStorage>());
        entityStorage = entityQuery.GetSingletonEntity();


    }
    // return the trigget that activates the spawner
    private void GetTrigger()
    {
        EntityQuery entityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<SpawnerTriggerComponent>());
        if(entityQuery.CalculateEntityCount() == 1)
        {
            spawnerTrigger = entityManager.GetComponentData<SpawnerTriggerComponent>( entityQuery.GetSingletonEntity());
            IsActive = spawnerTrigger.isActive;
        }

    }

    // get the list of enemies and their stats
    private DynamicBuffer<EnemyDataContainer> GetEnemyDataComponent()
    {
        EntityQuery entityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<EnemyDataContainer>());

        var dataContainer = entityManager.GetBuffer<EnemyDataContainer>(entityQuery.GetSingletonEntity());
        return dataContainer;
    }

    // spawns and initialises an enemy
    private void SpawnEntity(EnemyType enemyType)
    {
        PrefabEntityStorage pes = entityManager.GetComponentData<PrefabEntityStorage>(entityStorage);
        
        Entity e = Entity.Null;
        
        switch (enemyType)
        {
            case EnemyType.Melee:
                    e = entityManager.Instantiate(pes.MeleeEnemy);
                break;
            case EnemyType.Ranged:
                 e = entityManager.Instantiate(pes.RangedEnemy);
                break;
            case EnemyType.Bomb:
                e = entityManager.Instantiate(pes.BombEnemy);
                break;
        }

        // set random position
        Translation t = entityManager.GetComponentData<Translation>(e);

        t.Value = GetRandomPosition();
        entityManager.SetComponentData(e, t);

        IdleState i = new IdleState();

        i.EnemyDetectionRange = GetEnemyDataComponent()[((int)enemyType)].DetectionRange;

        entityManager.SetComponentData(e,i);

        // set enemies stats

        EnemyData enemyData = entityManager.GetComponentData<EnemyData>(e);

        enemyData.BaseHealth = GetEnemyDataComponent()[((int)enemyType)].Health;
        enemyData.CurrentHealth = GetEnemyDataComponent()[((int)enemyType)].Health;
        enemyData.Experience = GetEnemyDataComponent()[((int)enemyType)].Experience;
        enemyData.Gold = GetEnemyDataComponent()[((int)enemyType)].Gold;
            
        entityManager.SetComponentData(e, enemyData);
    }

    public void ActivateSpawner()
    {
        Debug.Log("Activating spawner");
        IsActive = true;
    }
    public bool CheckForLevelCleared()
    {
        EntityQuery q = entityManager.CreateEntityQuery(ComponentType.ReadOnly<EnemyTag>());


        if(EnemiesAmmount == 0 && q.IsEmpty)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // returns a random position in the spawning area
    private Vector3 GetRandomPosition()
    {
        Vector3 pos = new Vector3(Random.Range(this.transform.position.x - SpawnerSize.x/2, this.transform.position.x + SpawnerSize.x / 2),
            this.transform.position.y, Random.Range(this.transform.position.z - SpawnerSize.y / 2, this.transform.position.z + SpawnerSize.y / 2));

        return pos;
    }


    private EnemyType GetRandomEnemyType()
    {
        EnemyType e = EnemyTypes[Random.Range(0, EnemyTypes.Length)];
        return e;
    }
    private void OnDrawGizmos()
    {
        Vector3 cubesize = new Vector3(SpawnerSize.x, 1, SpawnerSize.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, cubesize);

    }

}
