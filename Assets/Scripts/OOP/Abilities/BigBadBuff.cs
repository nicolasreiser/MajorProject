using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class BigBadBuff : BaseAbility, IAbility
{
    private float CurrentCooldown;

    EntityManager entityManager;
    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

    }

    
}
