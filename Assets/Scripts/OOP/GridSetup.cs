using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class GridSetup : MonoBehaviour
{
    public static GridSetup Instance;

    public int Width;
    public int Height;
    public float CellSize;
    public Vector3 Origin;
    public GameObject prefab;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Origin = this.transform.position;
        PathfindingGrid grid = new PathfindingGrid(Width, Height, CellSize,Origin,prefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Vector3 cubesize = new Vector3(Width * CellSize, 1, Height * CellSize);
        Vector3 center = this.transform.position +new Vector3( cubesize.x/2,0,cubesize.z/2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, cubesize);
        
    }

}
