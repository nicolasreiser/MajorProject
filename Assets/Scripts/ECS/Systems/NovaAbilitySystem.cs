using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

public class NovaAbilitySystem : SystemBase
{
    SceneStorage sceneStorage;

    private bool isRunning;
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

        Entity projectile = Entity.Null;
        float deltaTime = Time.DeltaTime;

        Entities.
            WithNone<PausedTag>().
            ForEach((in PrefabEntityStorage prefabs) =>
            {
                projectile = prefabs.PlayerProjectile;
            }).Run();


        // check if this ability is in use

        Entities.ForEach((Entity entity, ref AbilityData abilityData) =>
        {
            if (abilityData.AbilityType == 3)
            {
                isActive = true;
            }

        }).Run();


        if (!isActive)
            return;


        //Get ability data

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
            if (abilityData.IsCast)
            {
                abilityData.IsCast = false;
                isCast = true;
                abilityData.Duration = abilityStorage[3].Duration;
                abilityData.Active = true;
            }

        }).Run();



        if (isCast)
        {
            isRunning = true;
        }

        if (isRunning)
        {
            CastNova(deltaTime, projectile, abilityStorage[3]);
        }
        // check if the ability is finished

        Entities.WithoutBurst()
            .ForEach((Entity entity, ref AbilityData abilityData) =>
        {
            if (abilityData.Duration <= 0 && abilityData.Active)
            {
                isRunning = false;
                abilityData.Active = false;
            }
        }).Run();

        
    }


    private void CastNova( float deltaTime, Entity projectile, AbilityStorageData abilityStorage)
    {
        bool shouldFire = false;

        Entities.ForEach((Entity entity, ref AbilityData abilityData) =>
        {
            abilityData.InternalCooldown -= deltaTime;

            if (abilityData.InternalCooldown <= 0)
            {
                shouldFire = true;
                abilityData.InternalCooldown = abilityStorage.IntervalCooldown;
            }

        }).Run();


        if(shouldFire)
        {

            Entities.
                WithNone<PausedTag>().
                WithStructuralChanges()
                .ForEach((Entity entity,
                int entityInQueryIndex,
                ref PlayerTag player,
                ref Translation translation,
                ref Rotation rotation,
                ref PhysicsVelocity physics,
                ref PlayerData playerData) =>
                {

                    Entity instance = EntityManager.Instantiate(projectile);

                    float3 offset = new float3(0, 1.0f, 1);

                    //random rotation

                    int randomNumber = UnityEngine.Random.Range(0, 180);

                    float angle = randomNumber * Mathf.PI * 2f / 180;
                    
                    Vector3 newPos = new Vector3(Mathf.Cos(angle+ 70* math.PI/180) * 1, 1.0f, Mathf.Sin(angle+ 70 * math.PI / 180) * 1);

                    float degAngle = (-angle + 20 * math.PI / 180) * 180 / Mathf.PI;
                    Quaternion offsetAngle = Quaternion.Euler(0, degAngle, 0);
                    Quaternion newAngle = rotation.Value * offsetAngle;

                    EntityManager.SetComponentData(instance, new Translation
                    {
                        
                        Value = new float3(translation.Value) + math.mul(rotation.Value, newPos)
                    });

                    EntityManager.SetComponentData(instance, new Rotation
                    {
                        Value = newAngle
                    });

                    bool ScatterShot = false;
                    float ScatterShotCooldown = 0;
                    int ScatterShotDamage = 0;

                    if (playerData.ScatterShot > 0)
                    {
                        ScatterShot = true;
                        ScatterShotCooldown = .3f;
                        ScatterShotDamage = ((playerData.WeaponBaseDamage * playerData.BulletDamagePercentage) / 400) * playerData.ScatterShot;
                    }

                    EntityManager.SetComponentData(instance, new BulletData
                    {
                        Damage = (int)(((playerData.WeaponBaseDamage * playerData.BulletDamagePercentage) / 100) * playerData.AttackDamageModifier),

                        ScatterShot = ScatterShot,
                        ScatterShotDamage = ScatterShotDamage,
                        ScatterShotCooldown = ScatterShotCooldown


                    });
                }).Run();

            
        }

    }
}
