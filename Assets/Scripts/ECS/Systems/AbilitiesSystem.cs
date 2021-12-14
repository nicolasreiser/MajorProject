using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class AbilitiesSystem : SystemBase
{
    //Ability Sorage Entity;

    protected override void OnUpdate()
    {

        // get the ability data

        DynamicBuffer<AbilityStorageData> abilityStorage = new DynamicBuffer<AbilityStorageData>();


        Entities.WithoutBurst().ForEach((in DynamicBuffer<AbilityStorageData> abilityDataContainer) =>
        {
            abilityStorage = abilityDataContainer;
        }
        ).Run();

        //EntityQuery query = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<DynamicBuffer<AbilityStorageData>>());

        //if(!query.IsEmpty)
        //{
          //  Debug.Log("Query not empty");
           // abilityStorage = EntityManager.GetBuffer<AbilityStorageData>(query.GetSingletonEntity());
        //}

        // check if in a run



        // check what ability is in use

        // check if ability is being cast


    }
}
