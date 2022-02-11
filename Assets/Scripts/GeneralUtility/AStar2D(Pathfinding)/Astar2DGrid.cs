using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar2DGrid : MonoBehaviour
{
    public Transform seeker, target;
    public LayerMask obstacleMask;
    public Vector3 gridWorldSize;
    public float nodeRadius;

    public List<Astar2DNode> path;

    Astar2DNode[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }
    // creates the grid array
    void CreateGrid()
    {
        grid = new Astar2DNode[gridSizeX, gridSizeY];
        // Vector3.right = (1,0,0) Vector3.up = (0,1,0) Vector3.forward = (0,0,1) etc...
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2
                                    - Vector3.up * gridWorldSize.y / 2;
        // draw 2D grid
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius)
                                        + Vector3.up * (y * nodeDiameter + nodeRadius);
                // specific solution for 2D - use sphere for 3D
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, obstacleMask));
                grid[x, y] = new Astar2DNode(walkable, worldPoint, x, y);
            }

        }
    }
    // return node position in grid array
    public Astar2DNode NodeFromWorldPoint(Vector3 worldPos)
    {
        // worldPos + (gridWorldSize/2) = current position
        float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;
        // make sure value is between 0 and 1
        // avoids errors if the target or seeker leaves the grid
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        // find the x and y position on the grid
        int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY-1) * percentY);

        return grid[x, y];
    }
    // return list of neighbour nodes of the input node
    public List<Astar2DNode> GetNeighbours(Astar2DNode node)
    {
        List<Astar2DNode> neighbours = new List<Astar2DNode>();
        // loops a 3x3 grid       x|x|x
        // setting the neighbours x|o|x
        // positions              x|x|x
        for (int x = -1; x <= 1; x++)
		{
	        for (int y = -1; y <= 1; y++)
			{
                if(x == 0 && y == 0)
                {
                    continue; // if equal to current node skip this iteration
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                // checks whether the neighbour node is in the grid array
                // if so then add it to the neighbour list
                if(checkX >= 0 && checkX < gridSizeX
                        && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
			}
		}
        return neighbours;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));
        if (grid != null)
        {
            Astar2DNode seekerNode = NodeFromWorldPoint(seeker.position);
            Astar2DNode targetNode = NodeFromWorldPoint(target.position);
            foreach (Astar2DNode n in grid)
            {
                // (n.walkable)? checks whether it is true, makes the box white or red
                // question the variable, the action taken is divided by a colon
                Gizmos.color = (n.walkable)?Color.white:Color.red;
                if (path != null)
                {
                    Debug.Log("path =/= null");
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                if (seekerNode == n || targetNode == n)
                {
                    Gizmos.color = Color.cyan;
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));

            }
        }
    }
}

