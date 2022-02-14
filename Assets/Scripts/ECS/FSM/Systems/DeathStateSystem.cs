using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


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

        Entities.WithoutBurst().
            ForEach((Entity entity,
            int entityInQueryIndex,
            ref EnemyTag enemy,
            ref DeathState deathState,
            ref EnemyData enemyData,
            ref LifetimeData lifetimeData) =>
            {

                // death animation maybe ?

                // add currency to player

                if(!enemyData.hasGivenGold)
                {
                    CurrencyManager currencyManager = CurrencyManager.Instance;
                    EntityQuery playerBuffQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<PlayerBuffComponent>());

                    var buffs = EntityManager.GetComponentData<PlayerBuffComponent>(playerBuffQuery.GetSingletonEntity());

                    currencyManager.AddGold((int)(enemyData.Gold * (1f + buffs.EarningsBuff/10f)));
                    enemyData.hasGivenGold = true;

                    //Debug.Log("Base Gold : "+ enemyData.Gold + " multiplyer : " + (1f + buffs.EarningsBuff / 10f));
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


                //Debug.Log("Enemy died");
                //lifetimeData.ShouldDie = true;
            }).Run();
    }
}
