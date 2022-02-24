using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

//  system iterating over enemies in  idle death state
public class DeathStateSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecb;

    private EntityQuery playerQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        
    }

    protected override void OnUpdate()
    {
        PauseManagement pm = PauseManagement.Instance;

        if (pm != null)
        {
            if (pm.IsPaused)
            {
                return;
            }
        }

        playerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<PlayerTag>(), ComponentType.ReadWrite<PlayerData>() }
        });

        var player = playerQuery.GetSingletonEntity();
        var commandBudder = ecb.CreateCommandBuffer();

        // get explosion prefab
        Entity explosion = Entity.Null;
        Entities.
            WithNone<PausedTag>().
            ForEach((in PrefabEntityStorage prefabs) =>
            {
                explosion = prefabs.DeathExplosion;
            }).Run();

        Entities.WithoutBurst().
            WithStructuralChanges().
            ForEach((Entity entity,
            int entityInQueryIndex,
            ref EnemyTag enemy,
            ref DeathState deathState,
            ref EnemyData enemyData,
            ref LifetimeData lifetimeData,
            in Translation translation) =>
            {
                // add currency to player
                if(!enemyData.hasGivenGold)
                {
                    CurrencyManager currencyManager = CurrencyManager.Instance;
                    EntityQuery playerBuffQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<PlayerBuffComponent>());

                    var buffs = EntityManager.GetComponentData<PlayerBuffComponent>(playerBuffQuery.GetSingletonEntity());

                    currencyManager.AddGold((int)(enemyData.Gold * (1f + buffs.EarningsBuff/10f)));
                    enemyData.hasGivenGold = true;

                }

                // add experience
                if(!enemyData.hasGivenExperience)
                {

                    PlayerData playerData = EntityManager.GetComponentData<PlayerData>(player);
                    playerData.OnExperienceChange = true;
                    playerData.Experience += enemyData.Experience;
                    if(playerData.Experience > playerData.MaxExperience)
                    {
                        playerData.OverflowExperience = playerData.Experience - playerData.MaxExperience;
                    }
                    enemyData.hasGivenExperience = true;

                    EntityManager.SetComponentData(player, playerData);
                }

                if(!enemyData.hasCastEffects)
                {

                var instance = EntityManager.Instantiate(explosion);

                EntityManager.SetComponentData(instance, new Translation
                {
                   Value = new float3(translation.Value)
                });
                    enemyData.hasCastEffects = true;
                }
            }).Run();
    }
}
