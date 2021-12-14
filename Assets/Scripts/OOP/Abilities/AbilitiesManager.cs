using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Transforms;

public class AbilitiesManager : MonoBehaviour
{
    public BaseAbility Ability;
    public Button AbilityButton;

    public AbilityScriptableObject[] AbilitySO;

    // Start is called before the first frame update
    void Awake()
    {
        CreateAbilityStorageEntity();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAbility(float name)
    {
        // set every ability to false

        // get ability wanted and set to true
    }

    public void CastAbility()
    {

    }

    public void IsOnCooldown()
    {

    }

    public void CreateAbilityStorageEntity()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = entityManager.CreateEntity(
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadWrite<AbilityStorageData>()
            );

        entityManager.SetName(entity, "AbilityStorageData");

        entityManager.SetComponentData(entity, new LocalToWorld
        {
            Value = new Unity.Mathematics.float4x4(rotation: Quaternion.identity, translation: new Unity.Mathematics.float3(0, 0, 0))
        });

        DynamicBuffer<AbilityStorageData> abilityStorage = entityManager.GetBuffer<AbilityStorageData>(entity);

        foreach (var item in AbilitySO)
        {
            abilityStorage.Add(new AbilityStorageData(item.Cooldown, item.Unlocked, item.Selected));
        }
    }

}

