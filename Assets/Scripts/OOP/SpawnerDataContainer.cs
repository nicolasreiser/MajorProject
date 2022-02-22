using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

// creates the dynakic buffer for the spawner data
public class SpawnerDataContainer : MonoBehaviour
{
    [SerializeField]
    private SpawnerDataScriptableObject spawnerData;

    EntityManager entityManager;

    void Start()
    {
        StartCoroutine(EntityGenerationCoroutine());
    }
    private bool GenerateEntity()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<LevelDataComponent>());
        if(query.IsEmpty)
        {
            return false;
        }

        var entity = query.GetSingletonEntity();
#if UNITY_EDITOR
        entityManager.SetName(entity, "LevelAndSpawnerData");
#endif

        entityManager.SetComponentData(entity, new LocalToWorld
        {
            Value = new Unity.Mathematics.float4x4(rotation: Quaternion.identity, translation: new Unity.Mathematics.float3(0, 0, 0))
        });

        entityManager.AddBuffer<SpawnerDataComponent>(entity);
        DynamicBuffer<SpawnerDataComponent> spawnerDataContainers = entityManager.GetBuffer<SpawnerDataComponent>(entity);


        for (int i = 0; i < spawnerData.spawnerData.Length; i++)
        {
            spawnerDataContainers.Add(new SpawnerDataComponent(spawnerData.spawnerData[i].EnemiesAmmount, spawnerData.spawnerData[i].InitialDelay,
                spawnerData.spawnerData[i].DelayBetweenSpawns, spawnerData.spawnerData[i].enemytypes));

        }

        return true;

    }

    private IEnumerator EntityGenerationCoroutine()
    {
        bool result = false;

        while(!result)
        {
            yield return new  WaitForFixedUpdate();

            result = GenerateEntity();
        }


    }

}

