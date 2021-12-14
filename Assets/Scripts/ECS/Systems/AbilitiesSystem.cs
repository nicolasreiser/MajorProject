using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class AbilitiesSystem : SystemBase
{
    SceneStorage sceneStorage;
    private float abilityCooldown;

    protected override void OnUpdate()
    {
        if (sceneStorage == null)
        {
            sceneStorage = SceneStorage.Instance;
        }

        // get the ability data

        DynamicBuffer<AbilityStorageData> abilityStorage = new DynamicBuffer<AbilityStorageData>();


        Entities.WithoutBurst().ForEach((in DynamicBuffer<AbilityStorageData> abilityDataContainer) =>
        {
            abilityStorage = abilityDataContainer;
        }
        ).Run();

        

        // check if in a run

        EntityQuery query = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<LevelDataComponent>());

        if (query.IsEmpty)
            return;
        
            LevelDataComponent ldc = EntityManager.GetComponentData<LevelDataComponent>(query.GetSingletonEntity());


        if (ldc.isMenu)
            return;

        // check what ability is in use

        var ability = new AbilityStorageData(); 

        foreach (var item in abilityStorage)
        {
            if(item.Selected)
            {
                ability = item;
                break;
            }
        }


        // check if ability is being cast

        if(ability.IsCast)
        {
            Debug.Log("Ability got cast");
            // do modifiers
        }

        // apply modifiers

    }
}
