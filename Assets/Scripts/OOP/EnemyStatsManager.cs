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

    public GameObject PlayerProjectile;
    public GameObject EnemyProjectile;
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
        entityManager.SetComponentData(entity, new EnemyDataContainer
        {
            MeleeDamage = MeleeEnemy.dammage,
            MeleeHealth = MeleeEnemy.health,
            MeleeRange = MeleeEnemy.Range,
            MeleeMaxRange = MeleeEnemy.MaxRange,
            MeleeCooldown = MeleeEnemy.WeaponCooldown,
            RangedDamage = RangedEnemy.dammage,
            RangedHealth = RangedEnemy.health,
            RangedRange = RangedEnemy.Range,
            RangedMaxRange = RangedEnemy.MaxRange,
            RangedCooldown = RangedEnemy.WeaponCooldown,
            BombDamage = BombEnemy.dammage,
            BombHealth = BombEnemy.health,
            BombRange = BombEnemy.Range,
            BombMaxRange = BombEnemy.MaxRange,
            BombCooldown = BombEnemy.WeaponCooldown
            
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
