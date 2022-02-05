using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class PlayerDeathSystem : SystemBase
{

    private bool HasCastDeathEffects;
    protected override void OnUpdate()
    {
        bool IsDead = false;
        Entities.ForEach((Entity entity, PlayerData playerData) =>
        {
            if(playerData.IsDead)
            {
                IsDead = true;
            }

        }).Run();

        if (IsDead && !HasCastDeathEffects)
        {
            CastDeathLoop();
        }

    }


    private void CastDeathLoop()
    {
        MonobehaviourStorageComponent monobehaviourStorageComponent = null;

        Entities.WithoutBurst().
          ForEach((Entity entity, MonobehaviourStorageComponent storage) =>
          {
              monobehaviourStorageComponent = storage;
          }).Run();

            Entities
                .WithoutBurst()
                .WithNone<PausedTag>()
                .ForEach((Entity entity, ref LevelDataComponent levelDataComponent ) =>
                {
                    levelDataComponent.ActivePlayer = false;
                }).Run();

        Entity explosion = Entity.Null;
        Entities.
            WithNone<PausedTag>().
            ForEach((in PrefabEntityStorage prefabs) =>
            {
                explosion = prefabs.DeathExplosion;
            }).Run();

        // Death effect

        Entities.
            WithStructuralChanges().
            ForEach((Entity entity, in PlayerData playerData, in Translation translation) =>
        {

            var instance = EntityManager.Instantiate(explosion);

            EntityManager.SetComponentData(instance, new Translation
            {
                Value = new float3(translation.Value)
            });

        }).Run();

        // death panel

        CanvasPanelManagement cpm = monobehaviourStorageComponent.MainCanvas.GetComponent<CanvasPanelManagement>();
        cpm.DeathPanelToggle();

        HasCastDeathEffects = true;
    }
}
