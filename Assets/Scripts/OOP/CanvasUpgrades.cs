using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using TMPro;
public class CanvasUpgrades : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private UpgradeList upgradesList;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private GameObject LevelCompletePanel;

    EntityManager entityManager;

    EntityQuery entity;
    EntityQuery playerStatsQuery;
    EntityQuery levelData;

    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entity = entityManager.CreateEntityQuery(ComponentType.ReadWrite<PlayerTag>());
        playerStatsQuery = entityManager.CreateEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<PlayerDataContainer>() }
        });
        levelData = entityManager.CreateEntityQuery(ComponentType.ReadWrite<LevelDataComponent>());

    }

    public void ToggleUI()
    {
        if (panel.activeSelf == true)
        {
            panel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
            SetupUpgrades();
        }
    }

    private void SetupUpgrades()
    {
        UpgradeScriptableObject upgrade1 = upgradesList.getRandomUpgrade();
        UpgradeScriptableObject upgrade2 = upgradesList.getRandomUpgrade();
        UpgradeScriptableObject upgrade3 = upgradesList.getRandomUpgrade();

        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
        button3.onClick.RemoveAllListeners();

        // Temporary

        button1.GetComponentInChildren<TextMeshProUGUI>().SetText(upgrade1.Name);
        button2.GetComponentInChildren<TextMeshProUGUI>().SetText(upgrade2.Name);
        button3.GetComponentInChildren<TextMeshProUGUI>().SetText(upgrade3.Name);
        //----
        switch (upgrade1.UpgradeType)
        {
            case UpgradeType.DoubleShot:
                button1.onClick.AddListener(() => DoubleBullet(upgrade1.PrimaryValue, upgrade1.SecondaryValue));
                break;
            case UpgradeType.ParallelShot:
                button1.onClick.AddListener(() => ParallelShot(upgrade1.PrimaryValue, upgrade1.SecondaryValue));
                break;
            case UpgradeType.ScatterShot:
                button1.onClick.AddListener(() => ScatterShot(upgrade1.PrimaryValue, upgrade1.SecondaryValue));
                break;
            case UpgradeType.DamageMultiplier:
                button1.onClick.AddListener(() => DamageAmplification(upgrade1.PrimaryValue, upgrade1.SecondaryValue));
                break;
            case UpgradeType.AttackSpeed:
                button1.onClick.AddListener(() => AttackSpeed(upgrade1.PrimaryValue, upgrade1.SecondaryValue));
                break;
        }
        switch (upgrade2.UpgradeType)
        {
            case UpgradeType.DoubleShot:
                button2.onClick.AddListener(() => DoubleBullet(upgrade2.PrimaryValue, upgrade2.SecondaryValue));
                break;
            case UpgradeType.ParallelShot:
                button2.onClick.AddListener(() => ParallelShot(upgrade2.PrimaryValue, upgrade2.SecondaryValue));
                break;
            case UpgradeType.ScatterShot:
                button2.onClick.AddListener(() => ScatterShot(upgrade2.PrimaryValue, upgrade2.SecondaryValue));
                break;
            case UpgradeType.DamageMultiplier:
                button2.onClick.AddListener(() => DamageAmplification(upgrade2.PrimaryValue, upgrade2.SecondaryValue));
                break;
            case UpgradeType.AttackSpeed:
                button2.onClick.AddListener(() => AttackSpeed(upgrade2.PrimaryValue, upgrade2.SecondaryValue));
                break;
        }
        switch (upgrade3.UpgradeType)
        {
            case UpgradeType.DoubleShot:
                button3.onClick.AddListener(() => DoubleBullet(upgrade3.PrimaryValue, upgrade3.SecondaryValue));
                break;
            case UpgradeType.ParallelShot:
                button3.onClick.AddListener(() => ParallelShot(upgrade3.PrimaryValue, upgrade3.SecondaryValue));
                break;
            case UpgradeType.ScatterShot:
                button3.onClick.AddListener(() => ScatterShot(upgrade3.PrimaryValue, upgrade3.SecondaryValue));
                break;
            case UpgradeType.DamageMultiplier:
                button3.onClick.AddListener(() => DamageAmplification(upgrade3.PrimaryValue, upgrade3.SecondaryValue));
                break;
            case UpgradeType.AttackSpeed:
                button3.onClick.AddListener(() => AttackSpeed(upgrade3.PrimaryValue, upgrade3.SecondaryValue));
                break;
        }

        button1.onClick.AddListener(ToggleUI);
        button2.onClick.AddListener(ToggleUI);
        button3.onClick.AddListener(ToggleUI);

        button1.onClick.AddListener(SetLevelData);
        button2.onClick.AddListener(SetLevelData);
        button3.onClick.AddListener(SetLevelData);
    }

    private void SetLevelData()
    {
        LevelDataComponent ldc = entityManager.GetComponentData<LevelDataComponent>(levelData.GetSingletonEntity());

        ldc.UpgradesToGet -= 1;
        ldc.Upgrading = false;
        if(ldc.UpgradesToGet == 0)
        {
            ldc.UpgradesReceived = true;
        }

        entityManager.SetComponentData(levelData.GetSingletonEntity(), ldc);
    }

    private void AddUpgrade()
    {
        LevelDataComponent ldc = entityManager.GetComponentData<LevelDataComponent>(levelData.GetSingletonEntity());

        ldc.UpgradesToGet += 1;
        entityManager.SetComponentData(levelData.GetSingletonEntity(), ldc);
        
    }

    public void LevelCompleted()
    {
        StartCoroutine(LevelCompletion());
    }
    private void SetReset()
    {
        LevelDataComponent ldc = entityManager.GetComponentData<LevelDataComponent>(levelData.GetSingletonEntity());

        ldc.ReadyForReset = true;

        entityManager.SetComponentData(levelData.GetSingletonEntity(), ldc);
    }

    private IEnumerator LevelCompletion()
    {
        LevelCompletePanel.SetActive(true);
        yield return new WaitForSeconds(2);

        LevelCompletePanel.SetActive(false);

        SetReset();
    }

    public void Heal()
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);
        playerData.CurrentHealth = playerData.BaseHealth;

        entityManager.SetComponentData(player, playerData);

    }

    public void LevelUp()
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);
        var Stats = entityManager.GetBuffer<PlayerDataContainer>(player);


        playerData.Experience = playerData.OverflowExperience;
        playerData.OverflowExperience = 0;
        playerData.Level++;
        playerData.BaseHealth = Stats[playerData.Level - 1].Health;
        playerData.MaxExperience = Stats[playerData.Level - 1].Experience;
        playerData.OnExperienceChange = true;

        entityManager.SetComponentData(player, playerData);

        AddUpgrade();

    }

    public void AddExp(int value)
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);
       
        playerData.Experience += value;
        playerData.OnExperienceChange = true;

        entityManager.SetComponentData(player, playerData);
    }

    public void DoubleBullet(int ammount, int multiplier)
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);

        playerData.EditDoubleSHot(ammount);
        playerData.EditDamageAmplification(multiplier);

        entityManager.SetComponentData(player, playerData);
    }
    public void ParallelShot(int ammount, int multiplier)
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);

        playerData.EditParallelShot(1);
        playerData.EditDamageAmplification(multiplier);

        entityManager.SetComponentData(player, playerData);
    }
    public void PiercingShot(int ammount, int multiplier)
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);

        playerData.EditPiercingShot(ammount);
        playerData.EditDamageAmplification(multiplier);

        entityManager.SetComponentData(player, playerData);
    }
    public void DamageAmplification(int ammount, int multiplier)
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);

        playerData.EditDamageAmplification(ammount);
        playerData.EditDamageAmplification(multiplier);

        entityManager.SetComponentData(player, playerData);
    }
    public void ScatterShot(int ammount, int multiplier)
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);

        playerData.EditScatterShot(ammount);
        playerData.EditDamageAmplification(multiplier);

        entityManager.SetComponentData(player, playerData);
    }
    public void AttackSpeed(int ammount, int multiplier)
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);

        playerData.EditAttackSpeed(ammount);
        playerData.EditDamageAmplification(multiplier);

        entityManager.SetComponentData(player, playerData);
    }
}
