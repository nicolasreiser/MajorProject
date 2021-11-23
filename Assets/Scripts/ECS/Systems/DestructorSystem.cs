using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class DestructorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.
            WithoutBurst().
            WithStructuralChanges().
            ForEach((Entity entity, in BulletData bulletData) =>
            {
                if(bulletData.ShouldDestroy)
                {
                    EntityManager.DestroyEntity(entity);
                }

            }).Run();
    }
}
