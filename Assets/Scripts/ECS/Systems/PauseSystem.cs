using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// system pausing the game when opening menus
public class PauseSystem : SystemBase
{
    protected override void OnUpdate()
    {
        PauseManagement pm = PauseManagement.Instance;

        if(pm != null)
        {
            if(pm.IsPaused)
            {
                //Debug.Log("Pausing");
                EntityManager e = EntityManager;

                Entities.
                    WithNone<PausedTag>().
                    WithStructuralChanges().
                    ForEach((Entity entity, PausableTag pausable) =>
                    {
                        PausedTag paused = new PausedTag();
                        e.AddComponentData(entity, paused);


                    }).Run();
            }
            else
            {
                //Debug.Log("Unpausing");
                EntityManager e = EntityManager;

                Entities.
                    WithAll<PausedTag>().
                    WithStructuralChanges().
                    ForEach((Entity entity, PausableTag pausable) =>
                    {
                        e.RemoveComponent<PausedTag>(entity);
                    }).Run();
            }
        }

    }
}
