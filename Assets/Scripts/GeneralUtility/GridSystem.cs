using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// brief: custom grid system 
public class GridSystem<T_GridObject>
{
    // event - when x or y values change
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    // private params
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private T_GridObject[,] gridArray;

    // debug params
    private bool showDebug;
    private TextMesh[,] debugTextArray;
    private int debugFontSize = 10;
    private Color debugColor = Color.white;
    private float debugTime = 100f;

    // constructor
    public GridSystem(int width, int height, float cellSize, Vector3 originPosition, Func<GridSystem<T_GridObject>, int, int, T_GridObject> createGridObject, bool showDebug)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.showDebug = showDebug;

        gridArray = new T_GridObject[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        if (showDebug)
        {
            debugTextArray = new TextMesh[width, height];
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = GeneralUtility.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, debugFontSize, debugColor, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), debugColor, debugTime);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), debugColor, debugTime);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), debugColor, debugTime);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), debugColor, debugTime);
            // same syntax as .js - everything after += is a function
            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
            {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }
    // event
    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridObjectChanged != null)
        {
            OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y }); // trigger the event
        }
    }

    // get
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
        return cellSize;
    }
    // get grid cell (x, y) from world position
    public void getXYfromWP(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }
    // get world position of grid cell
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }
    public T_GridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(T_GridObject);
        }
    }
    public T_GridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        getXYfromWP(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    // set
    public void SetGridObject(int x, int y, T_GridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            // gridArray[x, y] = Mathf.Clamp(value, HEAT_MAP_MIN_VALUE, HEAT_MAP_MAX_VALUE); // specific for heatmap
            gridArray[x, y] = value;
            if (OnGridObjectChanged != null)
            {
                OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y }); // trigger the event
            }
        }
    }
    public void SetGridObject(Vector3 worldPosition, T_GridObject value)
    {
        int x, y;
        getXYfromWP(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }
}
