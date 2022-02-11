using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar2DNode
{
    public bool walkable;

    public Vector3 worldPosition;

    public int gCost, hCost;

    public int gridX, gridY;

    public Astar2DNode parent;

    public Astar2DNode(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        walkable = _walkable;

        worldPosition = _worldPosition;

        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

}
