using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

// system enabled if the character has teh dash ability equipped

[UpdateAfter(typeof(PlayerMovementSystem))]
public class DashAbilitySystem : SystemBase
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

        // check if this ability is in use

        Entities.ForEach((Entity entity, ref AbilityData abilityData) =>
        {
            if (abilityData.AbilityType == 2)
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
                abilityData.Duration = abilityStorage[1].Duration;
                abilityData.Active = true;
            }

        }).Run();

        
        
        if (isCast)
        {
            isRunning = true;
        }

        if(isRunning)
        {
            DashStart();
        }

        // check if the ability is finished
        bool removeModifiers = false;
        Entities.
            WithoutBurst()
            .ForEach((Entity entity, ref AbilityData abilityData) =>
        {
            if (abilityData.Duration <= 0 && abilityData.Active)
            {
                isRunning = false;

                removeModifiers = true;
                abilityData.Active = false;
            }
        }).Run();

        if (removeModifiers)
        {
            DashFinish();
        }

    }


    private void DashStart()
    {
        //Debug.Log("Started Dashing");

        float deltaTime = Time.DeltaTime;
        Entities.
            WithAll<PlayerTag>().
            WithNone<PausedTag>().
            ForEach((ref PhysicsVelocity physics, ref PhysicsMass mass, ref Translation pos, ref Rotation rotation, ref MoveData moveData) =>
            {
                float2 newVel = physics.Linear.xz;

                 newVel += moveData.lastDirection.xz * moveData.speed * deltaTime * 50;
                
                physics.Linear.xz = newVel;
            }
            ).Run();
    }

    private void DashFinish()
    {
       // Debug.Log("Finished Dashing");

    }
}
