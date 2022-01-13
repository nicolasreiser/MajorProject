using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class SaveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityQuery query = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<LevelDataComponent>());

        if(query != null)
        {
            LevelDataComponent ldc = EntityManager.GetComponentData<LevelDataComponent>(query.GetSingletonEntity());

            if(ldc.ReadyForNextLevel && !ldc.hasSaved)
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

        PlayerStats stats = new PlayerStats();
        stats.Currency = CurrencyManager.Instance.Gold;
        stats.TopLevel = ldc.currentLevel + 1;
        //To change
        EntityQuery query = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<AbilityData>());
        AbilityData abilityData = EntityManager.GetComponentData<AbilityData>(query.GetSingletonEntity());

        stats.AbilityType = abilityData.AbilityType;

        SaveManager.SaveStats(stats,1);

    }
}
