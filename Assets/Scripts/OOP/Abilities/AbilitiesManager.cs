using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Transforms;

public class AbilitiesManager : MonoBehaviour
{
    public Button AbilityButton;
    public Image AbilityImage;
    public Image CooldownImage;
    public TMPro.TextMeshProUGUI CooldownText;


    public AbilityScriptableObject[] AbilitySO;
    private AbilityScriptableObject currentAbility;

    EntityManager entityManager;

    // Start is called before the first frame update
    void Awake()
    {
         entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        CreateAbilityStorageEntity();
        CreateAbilityDataEntity();

    }

    // Update is called once per frame
    void Update()
    {
        if(IsOnCooldown())
        {
            AbilityButton.interactable = false;
            CooldownText.enabled = true;
            SetAbilityCooldownUI();
        }
        else
        {
            AbilityButton.interactable = true;
            CooldownText.enabled = false;

        }


    }

    public void SelectAbility(AbilityType abilityType)
    {
        Sprite abilityImage = null; ;
        foreach (var ability in AbilitySO)
        {
            ability.Selected = false;
            if(ability.Ability == abilityType)
            {
                ability.Selected = true;
                abilityImage = ability.Picture;
                currentAbility = ability;
            }
        }
        // update UI TEMPORARY
        AbilityImage.sprite = abilityImage;

    }

    
    public void CreateAbilityStorageEntity()
    {
        var entity = entityManager.CreateEntity(
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadWrite<AbilityStorageData>()
            );

        entityManager.SetName(entity, "AbilityStorageDynamicBuffer");

        entityManager.SetComponentData(entity, new LocalToWorld
        {
            Value = new Unity.Mathematics.float4x4(rotation: Quaternion.identity, translation: new Unity.Mathematics.float3(0, 0, 0))
        });

        DynamicBuffer<AbilityStorageData> abilityStorage = entityManager.GetBuffer<AbilityStorageData>(entity);

        foreach (var item in AbilitySO)
        {
            abilityStorage.Add(new AbilityStorageData(item.Ability,item.Cooldown, item.Unlocked, item.Selected));
        }
    }

    public void CreateAbilityDataEntity()
    {
        var entity = entityManager.CreateEntity(
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadWrite<AbilityData>()
            
            );

        entityManager.SetName(entity, "AbilityStorageData");

        entityManager.SetComponentData(entity, new LocalToWorld
        {
            Value = new Unity.Mathematics.float4x4(rotation: Quaternion.identity, translation: new Unity.Mathematics.float3(0, 0, 0))
        });

        entityManager.SetComponentData(entity, new AbilityData
        {
            AbilityType = -1,
            BaseCooldown = 0,
            CurrentCooldown = 0,
            IsCast = false
        });
    }

    public void SetAbility( AbilityType abilityType)
    {
        EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<AbilityData>());
        AbilityData data = entityManager.GetComponentData<AbilityData>(query.GetSingletonEntity());

        switch (abilityType)
        {
            case AbilityType.BigBadBuff:
                data.AbilityType = 1;
                data.BaseCooldown = AbilitySO[0].Cooldown;
                break;
            case AbilityType.Dash:
                data.AbilityType = 2;
                data.BaseCooldown = AbilitySO[1].Cooldown;
                break;
            case AbilityType.Nova:
                data.AbilityType = 3;
                data.BaseCooldown = AbilitySO[3].Cooldown;
                break;
            default:
                break;
        }
        entityManager.SetComponentData(query.GetSingletonEntity(),data);
    }
    public void SetAbility1()
    {
        SetAbility(AbilityType.BigBadBuff);
    }

    public bool IsOnCooldown()
    {
        EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<AbilityData>());
        AbilityData data = entityManager.GetComponentData<AbilityData>(query.GetSingletonEntity());

        return data.CurrentCooldown <= 0 ? false : true;
    }

    public void CastAbility()
    {
        EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<AbilityData>());
        AbilityData data = entityManager.GetComponentData<AbilityData>(query.GetSingletonEntity());

        data.IsCast = true;
        data.CurrentCooldown = data.BaseCooldown;

        entityManager.SetComponentData(query.GetSingletonEntity(), data);
    }

    public void SetAbilityCooldownUI()
    {
        EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<AbilityData>());
        AbilityData data = entityManager.GetComponentData<AbilityData>(query.GetSingletonEntity());

        float value = 0;
        if (data.CurrentCooldown > 0)
        {
            value = data.CurrentCooldown / data.BaseCooldown;
        }

        CooldownImage.fillAmount = value;

        CooldownText.text = ((int)data.CurrentCooldown).ToString() ;
    }
}

