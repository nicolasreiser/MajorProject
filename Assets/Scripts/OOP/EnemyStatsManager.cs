using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class EnemyStatsManager : MonoBehaviour
{
    public EnemyStatsScriptableObject MeleeEnemy;
    public EnemyStatsScriptableObject RangedEnemy;
    public EnemyStatsScriptableObject BombEnemy;

    EntityManager entityManager;

    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = entityManager.CreateEntity(
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadWrite<EnemyDataContainer>()
            );


        entityManager.SetComponentData(entity, new LocalToWorld
        {
            Value = new Unity.Mathematics.float4x4(rotation: Quaternion.identity, translation: new Unity.Mathematics.float3(0, 0, 0))
        });

        DynamicBuffer<EnemyDataContainer> playerDataContainers = entityManager.GetBuffer<EnemyDataContainer>(entity);

        playerDataContainers.Add(new EnemyDataContainer(MeleeEnemy.health,MeleeEnemy.Range, MeleeEnemy.dammage,MeleeEnemy.MaxRange, MeleeEnemy.DetectionRange, MeleeEnemy.WeaponCooldown, MeleeEnemy.Experience));
        playerDataContainers.Add(new EnemyDataContainer(RangedEnemy.health, RangedEnemy.Range, RangedEnemy.dammage, RangedEnemy.MaxRange, RangedEnemy.DetectionRange, RangedEnemy.WeaponCooldown, RangedEnemy.Experience));
        playerDataContainers.Add(new EnemyDataContainer(BombEnemy.health, BombEnemy.Range, BombEnemy.dammage, BombEnemy.MaxRange, BombEnemy.DetectionRange, BombEnemy.WeaponCooldown, BombEnemy.Experience));

    }

}
