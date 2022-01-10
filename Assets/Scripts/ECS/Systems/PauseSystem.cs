using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PauseSystem : SystemBase
{
    protected override void OnUpdate()
    {
        PauseManagement pm = PauseManagement.Instance;

        if(pm != null)
        {
            if(pm.IsPaused)
            {
                Debug.Log("Pausing");
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
                Debug.Log("Unpausing");
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

        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("Pausing");
        //    EntityManager e = EntityManager;

        //    Entities.
        //        WithNone<PausedTag>().
        //        WithStructuralChanges().
        //        ForEach((Entity entity, PausableTag pausable) =>
        //        {
        //            PausedTag paused = new PausedTag();
        //            e.AddComponentData(entity, paused);


        //         }).Run();

        //}

        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    Debug.Log("Unpausing");
        //    EntityManager e = EntityManager;

        //    Entities.
        //        WithAll<PausedTag>().
        //        WithStructuralChanges().
        //        ForEach((Entity entity, PausableTag pausable) =>
        //        {
        //            e.RemoveComponent<PausedTag>(entity);
        //        }).Run();

        //}
    }
}
