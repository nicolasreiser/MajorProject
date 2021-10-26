using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public class CanvasUpgrades : MonoBehaviour
{
    EntityManager entityManager;

    EntityQuery entity;
    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entity = entityManager.CreateEntityQuery(ComponentType.ReadWrite<PlayerTag>());
    }

    // Update is called once per frame
    void Update()
    {
        
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
