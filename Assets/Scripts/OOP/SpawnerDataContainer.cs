using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class SpawnerDataContainer : MonoBehaviour
{
    [SerializeField]
    private SpawnerDataScriptableObject spawnerData;

    EntityManager entityManager;

    void Start()
    {
        GenerateEntity();
    }


    private void GenerateEntity()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<LevelDataComponent>());

        var entity = query.GetSingletonEntity();
        entityManager.SetName(entity, "LevelAndSpawnerData");

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

    }
}

