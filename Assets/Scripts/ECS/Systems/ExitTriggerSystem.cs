using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Burst;
using Unity.Transforms;

public class ExitTriggerSystem : JobComponentSystem
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
        public ComponentDataFromEntity<ExitTag> ExitGroup;
        public ComponentDataFromEntity<ExitTriggerComponent> ExitTriggerComponent;
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool isBodyATrigger = ExitGroup.HasComponent(entityA);
            bool isBodyBTrigger = ExitGroup.HasComponent(entityB);

            if (isBodyATrigger && isBodyBTrigger)
                return;


            bool isAPlayer = PlayerGroup.HasComponent(entityA);
            bool isBPlayer = PlayerGroup.HasComponent(entityB);

            if (!isAPlayer && !isBPlayer)
                return;

            var player = isAPlayer ? entityA : entityB;
            var trigger = isBodyATrigger ? entityA : entityB;

            if (!ExitTriggerComponent.HasComponent(trigger))
                return;
            
            var exitTrigger = ExitTriggerComponent[trigger];


            exitTrigger.Exit = true;

            ExitTriggerComponent[trigger] = exitTrigger;
            
            
           
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle jobHandle = new TriggerEventJob
        {
            PlayerGroup = GetComponentDataFromEntity<PlayerData>(),
            ExitGroup = GetComponentDataFromEntity<ExitTag>(),
            ExitTriggerComponent = GetComponentDataFromEntity<ExitTriggerComponent>()

        }.Schedule(m_StepPhysicsWorldSystem.Simulation,
                    ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);
        return jobHandle;
    }
}
