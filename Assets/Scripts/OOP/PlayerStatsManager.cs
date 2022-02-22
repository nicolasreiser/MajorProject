using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

// creates the player state entity
public class PlayerStatsManager : MonoBehaviour
{

    public PlayerStatsScriptableObject PlayerStats;

    EntityManager entityManager;


    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var entity = entityManager.CreateEntity(
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadWrite<PlayerDataContainer>()
            );

        entityManager.SetComponentData(entity, new LocalToWorld
        {
            Value = new Unity.Mathematics.float4x4(rotation: Quaternion.identity, translation: new Unity.Mathematics.float3(0, 0, 0))
        });

        DynamicBuffer<PlayerDataContainer> playerDataContainers = entityManager.GetBuffer<PlayerDataContainer>(entity);


        for (int i = 0; i < PlayerStats.Health.Length; i++)
        {
            playerDataContainers.Add(new PlayerDataContainer(PlayerStats.Health[i],PlayerStats.Experience[i]));

        }

    }

}
