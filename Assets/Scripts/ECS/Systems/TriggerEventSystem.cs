using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Burst;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]

public class TriggerEventSystem : JobComponentSystem
{

    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    StepPhysicsWorld m_StepPhysicsWorldSystem;


    protected override void OnCreate()
    {
        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    }



    //[BurstCompile]
    struct TriggerEventJob : ITriggerEventsJob
    {

        public ComponentDataFromEntity<PlayerData> PlayerGroup;
        public ComponentDataFromEntity<TriggerTag> TriggerGroup;
        public ComponentDataFromEntity<SpawnerTriggerComponent> TriggerComponent;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool isBodyATrigger = TriggerGroup.HasComponent(entityA);
            bool isBodyBTrigger = TriggerGroup.HasComponent(entityB);

            if(isBodyATrigger && isBodyBTrigger)
                return;
            

            bool isAPlayer = PlayerGroup.HasComponent(entityA);
            bool isBPlayer = PlayerGroup.HasComponent(entityB);

            if (!isAPlayer && !isBPlayer)
                return;

            var player = isAPlayer ? entityA : entityB;
            var trigger = isBodyATrigger ? entityA : entityB;

            if (!TriggerComponent.HasComponent(trigger))
                return;

            //var spawners = Object.FindObjectsOfType<EnemiesSpawner>();
            var t = TriggerComponent[trigger];
            t.isActive = true;
            TriggerComponent[trigger] = t;
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle jobHandle = new TriggerEventJob
        {
            PlayerGroup = GetComponentDataFromEntity<PlayerData>(),
            TriggerGroup = GetComponentDataFromEntity<TriggerTag>(),
            TriggerComponent = GetComponentDataFromEntity<SpawnerTriggerComponent>()
        
        }.Schedule(m_StepPhysicsWorldSystem.Simulation,
                    ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);
         return jobHandle;
    }
}
