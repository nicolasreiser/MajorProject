using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;



public class PlayerStatsSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecb;

    private EntityQuery playerStatsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        playerStatsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<PlayerDataContainer>() }
        });

    }

    protected override void OnUpdate()
    {
        UpdateStats();
        PlayerInvulnerability();
        ActivePlayer();
    }

    private void UpdateStats()
    {
        
        var player = playerStatsQuery.GetSingletonEntity();

        var Stats = EntityManager.GetBuffer<PlayerDataContainer>(player);
        

        Entities.
            WithoutBurst().
            WithAll<PlayerTag>().
            WithNone<PausedTag>().
            ForEach((Entity entity, ref PlayerData playerData) =>
            {
                if (playerData.Experience >= playerData.MaxExperience || !playerData.Initialised)
                {
                    playerData.Experience = playerData.OverflowExperience;
                    playerData.OverflowExperience = 0;
                    playerData.Level++;
                    playerData.BaseHealth = Stats[playerData.Level - 1].Health;

                    var healthToAdd = 0;
                    if(playerData.Initialised)
                    {
                        healthToAdd = playerData.BaseHealth - Stats[playerData.Level - 2].Health;
                    }
                    playerData.CurrentHealth += healthToAdd;
                    playerData.MaxExperience = Stats[playerData.Level - 1].Experience;
                    playerData.OnExperienceChange = true;
                    playerData.OnHealthChange = true;
                    if(!playerData.Initialised)
                    {
                        playerData.CurrentHealth = playerData.BaseHealth;
                        Debug.Log("Player Initialised");

                    }

                    if (playerData.Initialised)
                    {
                        AddLevel();
                    }
                    playerData.Initialised = true;
                
                }

            }).Run();
    }

    private void AddLevel()
    {
        Entities.
            ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
            {
                levelDataComponent.UpgradesToGet += 1;
            }).Run();
    }

    private void PlayerInvulnerability()
    {
        EntityQuery playerQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<LevelDataComponent>());
        if (playerQuery.IsEmpty)
            return;

        var ldc = EntityManager.GetComponentData<LevelDataComponent>(playerQuery.GetSingletonEntity());
        Entities.
            WithAll<PlayerTag>().
            WithNone<PausedTag>().
            ForEach((Entity entity, ref PlayerData playerData) =>
            {
                playerData.IsInvulnerable = ldc.PlayerInvulnerability;
            }).Run();
    }

    private void ActivePlayer()
    {
        Entity player = Entity.Null;
        EntityQuery playerQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerData>());
        if (!playerQuery.IsEmpty)
        {
            player = playerQuery.GetSingletonEntity();
        }

        if(player == Entity.Null)
        {
            return;
        }
        Debug.Log("Player : " + player);
        Entities.
            WithStructuralChanges().
            WithNone<PausedTag>().
           ForEach((Entity entity, ref LevelDataComponent levelDataComponent) =>
           {
               if (!levelDataComponent.ActivePlayer)
               {
                   EntityManager.SetEnabled(player, false);
               }
               else
               {
                   EntityManager.SetEnabled(player, true);
               }
           }).Run();
    }
}
