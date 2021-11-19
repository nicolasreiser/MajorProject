using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class EnemiesSpawner : MonoBehaviour
{

    public Vector2 SpawnerSize;
    public int EnemiesAmmount;
    public EnemyType[] EnemyTypes;
    public float InitialDelay;
    public float DelayBetweenSpawns;
    private float currentDelayBetweenSpawns;

    private EntityManager entityManager;
    private Camera uiCamera;
    private Canvas uiCanvas;



    private Entity entityStorage;
    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        currentDelayBetweenSpawns = 0;
        GetPrefabs();
        
    }

    // Update is called once per frame
    void Update()
    {
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


    private void GetPrefabs()
    {
        EntityQuery entityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PrefabEntityStorage>());
        entityStorage = entityQuery.GetSingletonEntity();
    }
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

        Translation t = entityManager.GetComponentData<Translation>(e);

        t.Value = GetRandomPosition();
        entityManager.SetComponentData<Translation>(e, t);
        
    }

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
