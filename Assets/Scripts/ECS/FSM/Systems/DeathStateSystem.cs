using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


public class DeathStateSystem : SystemBase
{


    protected override void OnUpdate()
    {

        Entities.
            ForEach((Entity entity,
            int entityInQueryIndex,
            ref EnemyTag enemy,
            ref DeathState deathState,
            ref LifetimeData lifetimeData) =>
            {
                // death animation

                // add currency to player


                Debug.Log("Enemy died");
                //lifetimeData.ShouldDie = true;
            }).ScheduleParallel();
    }
}
