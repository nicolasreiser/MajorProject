using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class BigBadBuff : BaseAbility, IAbility
{
    private float CurrentCooldown;

    EntityManager entityManager;
    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentCooldown -= Time.deltaTime;
    }

    public void Initialize()
    {
        AbilityStorageData currentAbility  = GetCurrentAbilityData();


        BaseCooldown = currentAbility.Cooldown;

        
    }
    public override void CastAbility()
    {
        if(CurrentCooldown <= 0)
        {
            AbilityStorageData currentAbility = GetCurrentAbilityData();
            currentAbility.IsCast = true;
        }

        // go full ecs with multiple systems and imtermediate datacomponents

    }

    public override bool IsOnCooldown()
    {

        return false;
    }

    private AbilityStorageData GetCurrentAbilityData()
    {
        EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DynamicBuffer<AbilityStorageData>>());
        if (!query.IsEmpty)
        {
            DynamicBuffer<AbilityStorageData> abilityStorage = entityManager.GetBuffer<AbilityStorageData>(query.GetSingletonEntity());

            AbilityStorageData currentAbility = new AbilityStorageData();
            foreach (var item in abilityStorage)
            {
                if (item.Selected)
                {
                    currentAbility = item;
                    break;
                }
            }

            return currentAbility;
        }
        return new AbilityStorageData();

    }
    
}
