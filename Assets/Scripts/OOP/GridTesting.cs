using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTesting : MonoBehaviour
{
    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        PathfindingGrid grid = new PathfindingGrid(5, 5, 1,Vector3.zero,prefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
