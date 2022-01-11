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

                // death animation

                // add currency to player

                if(!enemyData.hasGivenGold)
                {
                    CurrencyManager currencyManager = CurrencyManager.Instance;

                    currencyManager.AddGold(enemyData.Gold);
                    enemyData.hasGivenGold = true;

                    Debug.Log("Added Gold");
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
