using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public class CanvasUpgrades : MonoBehaviour
{
    EntityManager entityManager;

    EntityQuery entity;
    EntityQuery playerStatsQuery;
    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entity = entityManager.CreateEntityQuery(ComponentType.ReadWrite<PlayerTag>());
        playerStatsQuery = entityManager.CreateEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<PlayerDataContainer>() }
        });
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

    }

    public void AddExp(int value)
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);
       
        playerData.Experience += value;
        playerData.OnExperienceChange = true;

        entityManager.SetComponentData(player, playerData);
    }

    public void DoubleBullet()
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);

        playerData.EditDoubleSHot(1);

        entityManager.SetComponentData(player, playerData);
    }
    public void ParallelShot()
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);

        playerData.EditParallelShot(1);

        entityManager.SetComponentData(player, playerData);
    }
    public void PiercingShot()
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);

        playerData.EditPiercingShot(1);

        entityManager.SetComponentData(player, playerData);
    }
    public void DamageAmplification()
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);

        playerData.EditDamageAmplification(25);

        entityManager.SetComponentData(player, playerData);
    }
    public void ScatterShot()
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);

        playerData.EditScatterShot(1);

        entityManager.SetComponentData(player, playerData);
    }
    public void AttackSpeed()
    {
        var player = entity.GetSingletonEntity();
        PlayerData playerData = entityManager.GetComponentData<PlayerData>(player);

        playerData.EditAttackSpeed(25);

        entityManager.SetComponentData(player, playerData);
    }
}
