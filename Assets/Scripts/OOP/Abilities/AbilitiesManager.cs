using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Transforms;

public class AbilitiesManager : MonoBehaviour
{
    public Button AbilityButton;
    public GameObject AbilityPanel;
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
        InitialiseAbility();
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


        // DEBUG

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CastAbility();
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
#if UNITY_EDITOR
        entityManager.SetName(entity, "AbilityStorageDynamicBuffer");
#endif

        entityManager.SetComponentData(entity, new LocalToWorld
        {
            Value = new Unity.Mathematics.float4x4(rotation: Quaternion.identity, translation: new Unity.Mathematics.float3(0, 0, 0))
        });

        DynamicBuffer<AbilityStorageData> abilityStorage = entityManager.GetBuffer<AbilityStorageData>(entity);

        foreach (var item in AbilitySO)
        {
            switch (item.Ability)
            {
                case AbilityType.BigBadBuff:

                    BBBAbilityScriptableObject bbb = item as BBBAbilityScriptableObject;
                    abilityStorage.Add(new AbilityStorageData(bbb.Ability, bbb.Cooldown, bbb.Unlocked, bbb.Selected,bbb.AttackDamageBuff,bbb.AttackSpeedBuff, bbb.Duration));

                    break;
                case AbilityType.Dash:
                    DashAbilityScriptableObject dash = item as DashAbilityScriptableObject;
                    abilityStorage.Add(new AbilityStorageData(dash.Ability, dash.Cooldown, dash.Unlocked, dash.Selected, dash.Duration));

                    break;
                case AbilityType.Nova:

                    NovaScriptableObject nso = item as NovaScriptableObject;
                    abilityStorage.Add(new AbilityStorageData(nso.Ability, nso.Cooldown, nso.Unlocked, nso.Selected, nso.Duration, nso.InternalCooldown));
                    break;
                default:
                    break;
            }
            //abilityStorage.Add(new AbilityStorageData(item.Ability,item.Cooldown, item.Unlocked, item.Selected));
        }
    }

    public void CreateAbilityDataEntity()
    {
        var entity = entityManager.CreateEntity(
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadWrite<AbilityData>()
            
            );
#if UNITY_EDITOR
        entityManager.SetName(entity, "AbilityStorageData");
#endif

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
                AbilityImage.sprite = AbilitySO[0].Picture;
                break;
            case AbilityType.Dash:
                data.AbilityType = 2;
                data.BaseCooldown = AbilitySO[1].Cooldown;
                AbilityImage.sprite = AbilitySO[1].Picture;

                break;
            case AbilityType.Nova:
                data.AbilityType = 3;
                data.BaseCooldown = AbilitySO[2].Cooldown;
                AbilityImage.sprite = AbilitySO[2].Picture;

                break;
            default:
                data.AbilityType = 0;
                break;
        }

        entityManager.SetComponentData(query.GetSingletonEntity(),data);
    }
    public void SetAbility1()
    {
        SetAbility(AbilityType.BigBadBuff);
        InitialiseAbility();
    }
    public void SetAbility2()
    {
        SetAbility(AbilityType.Dash);
        InitialiseAbility();
    }
    public void SetAbility3()
    {
        SetAbility(AbilityType.Nova);
        InitialiseAbility();
    }

    private void InitialiseAbility()
    {

        //todo, wait for save to be loaded and it has to be start level

        StartCoroutine(InitialisationCoroutine());
    }
    private IEnumerator InitialisationCoroutine()
    {
        EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<LevelDataComponent>());

        LevelDataComponent ldc = new LevelDataComponent();
        while (query.CalculateEntityCount() == 0)
        {
            query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<LevelDataComponent>());
            
            yield return new WaitForFixedUpdate();
        }

        ldc = entityManager.GetComponentData<LevelDataComponent>(query.GetSingletonEntity());

        while (!ldc.hasLoadedSave || !ldc.isStartLevel)
        {
            yield return new WaitForFixedUpdate();
        }

        EntityQuery abilityQuery = entityManager.CreateEntityQuery(ComponentType.ReadWrite<AbilityData>());
        AbilityData data = entityManager.GetComponentData<AbilityData>(abilityQuery.GetSingletonEntity());

        if (data.AbilityType <= 0)
        {
            Debug.Log("No ability selected");
            AbilityPanel.SetActive(false);
        }
        else
        {
            AbilityPanel.SetActive(true);

            AbilityType type = (AbilityType)data.AbilityType;
            Debug.Log("Ability " + type + " selected");

            SetAbility(type);
        }
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

