using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class BigBadBuffAbilitySystem : SystemBase
{
    SceneStorage sceneStorage;

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

        bool isActive = false;
        if (sceneStorage == null)
        {
            sceneStorage = SceneStorage.Instance;
        }


        var deltaTime = Time.DeltaTime;
        Entities.ForEach((Entity entity, ref AbilityData abilityData) =>
        {
            abilityData.CurrentCooldown -= deltaTime;
            abilityData.Duration -= deltaTime;
            if (abilityData.CurrentCooldown < 0)
                abilityData.CurrentCooldown = 0;
            if (abilityData.Duration < 0)
                abilityData.Duration = 0;
        }).Run();

        // check if this ability is in use

        //EntityQuery ldcquery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<AbilityData>());

        Entities.ForEach((Entity entity, ref AbilityData abilityData) =>
        {
            if (abilityData.AbilityType == 1)
            {
                isActive = true;
            }

        }).Run();

        if (!isActive)
            return;

        // get the ability data

        DynamicBuffer<AbilityStorageData> abilityStorage = new DynamicBuffer<AbilityStorageData>();


        Entities.WithoutBurst().ForEach((Entity entity, in DynamicBuffer<AbilityStorageData> abilityDataContainer) =>
        {
            abilityStorage = abilityDataContainer;
        }
        ).Run();

        

        // check if in a run

        EntityQuery ldcquery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<LevelDataComponent>());

        if (ldcquery.IsEmpty)
            return;
        
       LevelDataComponent ldc = EntityManager.GetComponentData<LevelDataComponent>(ldcquery.GetSingletonEntity());


        if (ldc.isMenu)
            return;

        // check if ability is being cast

        bool isCast = false;
        Entities.ForEach((Entity entity, ref AbilityData abilityData) =>
        {
            if(abilityData.IsCast)
            {
                abilityData.IsCast = false;
                isCast = true;
                abilityData.Duration = abilityStorage[1].Duration;
                abilityData.Active = true;
            }

        }).Run();

        if (isCast)
        {
            // apply modifiers
            Entities.
                WithoutBurst().
                ForEach((Entity entity, ref PlayerData playerData) =>
            {
                ApplyModifiers(ref  playerData, abilityStorage[1]);

            }).Run();

        }

        // check if the ability is finished
        bool removeModifiers = false;
        Entities.ForEach((Entity entity, ref AbilityData abilityData) =>
        {
            if(abilityData.Duration <= 0 && abilityData.Active)
            {
                abilityData.Active = false;
                removeModifiers = true;
            }
        }).Run();

        if(removeModifiers)
        {
            Entities.
                WithoutBurst().
                ForEach((Entity entity, ref PlayerData playerData) =>
                {
                    RemoveModifiers(ref playerData);

                }).Run();
        }
    }

    private void ApplyModifiers( ref PlayerData playerData, AbilityStorageData abilityStorageData)
    {
        Debug.Log("Set the modifiers");

        playerData.AttackSpeedModifier = abilityStorageData.AttackspeedModifier;
        playerData.AttackDamageModifier = abilityStorageData.DamageModifier;

    }
    private void RemoveModifiers(ref PlayerData playerData)
    {
        Debug.Log("Removed the modifiers");
        playerData.AttackSpeedModifier = 1;
        playerData.AttackDamageModifier = 1;

    }
}
