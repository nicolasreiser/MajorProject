using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid 
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    public PathfindingGrid(int width, int height, float cellSize, Vector3 origin, GameObject displayPrefab)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new int[width, height];

        Debug.Log(width + " " + height);

        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                Vector3 spawnposition = origin + new Vector3(i + cellSize/2,0, j + cellSize/2) * cellSize;

                Object.Instantiate(displayPrefab, spawnposition,Quaternion.identity);
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize;
    }
}
