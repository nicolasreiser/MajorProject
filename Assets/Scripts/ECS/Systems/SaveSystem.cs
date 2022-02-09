using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class SaveSystem : SystemBase
{
    PlayerStats stats;

    protected override void OnCreate()
    {
        stats = new PlayerStats();
    }

    protected override void OnUpdate()
    {
        EntityQuery query = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<LevelDataComponent>());

        if(!query.IsEmpty)
        {
            LevelDataComponent ldc = EntityManager.GetComponentData<LevelDataComponent>(query.GetSingletonEntity());

            if (ldc.ReadyForNextLevel && !ldc.hasLoadedSave)
            {
                Load();
                ldc.hasLoadedSave = true;
                EntityManager.SetComponentData<LevelDataComponent>(query.GetSingletonEntity(), ldc);
            }

            ldc = EntityManager.GetComponentData<LevelDataComponent>(query.GetSingletonEntity());

            if (ldc.ReadyForNextLevel && !ldc.hasSaved)
            {
                //Todo save mechanic
                Save(ldc);
                ldc.hasSaved = true;
                EntityManager.SetComponentData<LevelDataComponent>(query.GetSingletonEntity(), ldc);
            }


            
        }

        

    }


    private void Save(LevelDataComponent ldc)
    {
        Debug.Log("Saving...");

        stats.RunCurrency = CurrencyManager.Instance.Gold;
        stats.LastLevel = ldc.currentLevel + 1;

        //To change
        EntityQuery query = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<AbilityData>());
        if(!query.IsEmpty)
        {
            AbilityData abilityData = EntityManager.GetComponentData<AbilityData>(query.GetSingletonEntity());
            stats.AbilityType = abilityData.AbilityType;

        }


        SaveManager.SaveStats(stats);

    }

    private void Load()
    {
        SaveData saveData = SaveManager.LoadStats();

        stats.TotalCurrency = saveData.Currency;
        stats.AbilityType = saveData.AbilityType;

        stats.HealthBuff = saveData.HealthBuff;
        stats.DamageBuff = saveData.DamageBuff;
        stats.AttackspeedBuff = saveData.AttackspeedBuff;
        stats.EarningsBuff = saveData.EarningsBuff;

        stats.AbilitiesLock = new List<bool>();

        foreach (bool item in saveData.AbilitiesUnlocked)
        {
            stats.AbilitiesLock.Add(item); 

        }

        EntityQuery query = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<AbilityData>());
        if (!query.IsEmpty)
        {
            AbilityData abilityData = EntityManager.GetComponentData<AbilityData>(query.GetSingletonEntity());
            abilityData.AbilityType = stats.AbilityType;

            EntityManager.SetComponentData(query.GetSingletonEntity(), abilityData);
        }
        
        stats.TopLevel = saveData.TopLevel;


        // create playerbuff entity

        EntityQuery playerBuffQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<PlayerBuffComponent>());

        
            Entity e = EntityManager.CreateEntity(
            ComponentType.ReadOnly<LocalToWorld>()
            );
            EntityManager.AddComponentData(e, new PlayerBuffComponent {
                AttackspeedBuff = 1,
                EarningsBuff = 0,
                DamageBuff= 0,
                HealthBuff = 0

            });

        
        //playerBuffQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<PlayerBuffComponent>());


        PlayerBuffComponent playerData = EntityManager.GetComponentData<PlayerBuffComponent>(e);
            playerData.HealthBuff = saveData.HealthBuff;
            playerData.DamageBuff = saveData.DamageBuff;
            playerData.AttackspeedBuff = saveData.AttackspeedBuff;
            playerData.EarningsBuff = saveData.EarningsBuff;


        Debug.Log("PlayerBuffEntity : " +  playerData.AttackspeedBuff);
            EntityManager.SetComponentData(e, playerData);
        

        Debug.Log("Loaded...");
    }
}
