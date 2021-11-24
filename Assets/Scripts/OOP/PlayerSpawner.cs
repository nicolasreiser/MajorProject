using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class PlayerSpawner : MonoBehaviour
{
    private EntityManager entityManager;
    private Entity entityStorage;


    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PrefabEntityStorage>());
        entityStorage = entityQuery.GetSingletonEntity();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPlayer()
    {
        PrefabEntityStorage pes = entityManager.GetComponentData<PrefabEntityStorage>(entityStorage);


        var playerEntity = entityManager.Instantiate(pes.Player);

        // todo set spawn position
        Translation t = entityManager.GetComponentData<Translation>(playerEntity);

        t.Value = new Vector3(2, 4, -7);
        entityManager.SetComponentData(playerEntity, t);
    }

    public void FreezePlayer(bool state)
    {

    }

}
