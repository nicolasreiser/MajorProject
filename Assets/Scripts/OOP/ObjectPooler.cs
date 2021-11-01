using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public GameObject ObjectToPool;
    public Transform Parent;
    public int BasePoolAmmount;

    private List<GameObject> pooledObjects;
    private List<GameObject> objectsInUse;

    public static ObjectPooler Instance;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
            Instance = this;
        
        pooledObjects = new List<GameObject>();
        objectsInUse = new List<GameObject>();

        for (int i = 0; i < BasePoolAmmount; i++)
        {
            GameObject obj = GameObject.Instantiate(ObjectToPool,Vector3.zero,Quaternion.identity,Parent);
            obj.SetActive(false);
            pooledObjects.Add(obj);

        }
    }


    public GameObject getPooledObject()
    {
        Debug.Log("getpooledobject");
        if(pooledObjects.Count == 0)
        {
            AddObjectsToPool(2);
        }
        GameObject obj = pooledObjects[0];
        obj.SetActive(true);
        pooledObjects.Remove(obj);
        objectsInUse.Add(obj);

        Debug.Log("Sending object : " + obj);
        return obj;
    }
    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);

        objectsInUse.Remove(obj);
        pooledObjects.Add(obj);
    }
    private void AddObjectsToPool(int value)
    {
        for (int i = 0; i < value; i++)
        {
            GameObject obj = Object.Instantiate(ObjectToPool, Vector3.zero, Quaternion.identity, Parent);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }
}
