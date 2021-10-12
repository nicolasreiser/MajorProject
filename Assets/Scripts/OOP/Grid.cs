using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid<TGridObject> 
{
    private int width;
    private int height;
    private float celllSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.celllSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

    }

    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }
    public float GetCellSize()
    {
        return celllSize;
    }
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * celllSize + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / celllSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / celllSize);
    }

    public void SetValue(int x, int y, TGridObject value)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
        }
    }

    public void SetValue(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public TGridObject GetValue( int x, int y)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetValue(Vector3 worldPosition)
    {
        int x,y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);

    }
}
