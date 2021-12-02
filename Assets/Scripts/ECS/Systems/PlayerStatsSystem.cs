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

        //UpdateStats();

        //SetHealth();

    }

    protected override void OnUpdate()
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        
        var player = playerStatsQuery.GetSingletonEntity();

        var Stats = EntityManager.GetBuffer<PlayerDataContainer>(player);
        

        Entities.
            WithoutBurst().
            WithAll<PlayerTag>().
            ForEach((Entity entity, ref PlayerData playerData) =>
            {
                //Debug.Log($"Current Exp : {playerData.Experience} Max Exp : {playerData.MaxExperience}");
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
                        playerData.CurrentHealth = playerData.BaseHealth;
                    
                    if(playerData.Initialised)
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
}
