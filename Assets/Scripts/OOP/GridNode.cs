using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// nodes covering the grid and defining if an area is walkable or not
public class GridNode
{
    private Grid<GridNode> grid;
    private int x;
    private int y;

    private bool isWalkable;

    public GridNode(Grid<GridNode> grid,int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
}
