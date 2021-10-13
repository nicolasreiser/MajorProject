using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class GridSetup : MonoBehaviour
{
    public static GridSetup Instance;

    public int Width;
    public int Height;
    public float CellSize;
    public Vector3 Origin;
    public GameObject prefab;

    public Grid<GridNode> pathfindingGrid;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Origin = this.transform.position;
        pathfindingGrid = new Grid<GridNode>(Width, Height, CellSize,Origin, (Grid<GridNode> grid, int x, int y) => new GridNode(grid, x, y));
        pathfindingGrid.AddVisual(prefab);
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

    public int2 GetGridPosition(float3 worldPosition)
    {
        int x;
        int y;
        pathfindingGrid.GetXY(worldPosition, out x,out y);
        int2 gridPosition = new int2(x, y);
        return gridPosition;
    }
}
