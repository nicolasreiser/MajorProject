using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class CameraMovement : MonoBehaviour
{
    private new Camera camera;
    private EntityManager entityManager;
    public Vector3 Offset;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos;
        Vector3 playerPosition = GetPlayerPosition();
        pos = playerPosition + Offset;

        camera.transform.position = Vector3.Slerp(camera.transform.position, pos, Time.deltaTime);
    }

    private Vector3 GetPlayerPosition()
    {

        EntityQuery player = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerTag>(), ComponentType.ReadOnly<Translation>());

        Translation t = entityManager.GetComponentData<Translation>(player.GetSingletonEntity());

        return new Vector3(t.Value.x, t.Value.y, t.Value.z);
    }

}
