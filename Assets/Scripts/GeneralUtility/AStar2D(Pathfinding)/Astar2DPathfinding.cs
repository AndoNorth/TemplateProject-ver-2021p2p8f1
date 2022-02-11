using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar2DPathfinding : MonoBehaviour
{
    public Transform seeker, target;

    Astar2DGrid grid;

    void Awake(){
        grid = GetComponent<Astar2DGrid>();
    }

    void Update(){
        FindPath(seeker.position, target.position);
    }
    // Astar pathfinding
    void FindPath(Vector3 startPos, Vector3 targetPos){
        Astar2DNode startNode = grid.NodeFromWorldPoint(startPos);
        Astar2DNode targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Astar2DNode> openSet = new List<Astar2DNode>(); // set of nodes to be assessed
        HashSet<Astar2DNode> closedSet = new HashSet<Astar2DNode>(); // set of nodes that have been assessed

        openSet.Add(startNode); // initialise the open list with the startNode

        while (openSet.Count > 0)
        {
            Astar2DNode currentNode = openSet[0]; // set currentNode to the first node in the open list

            // loop the remainder of the open list
            for (int i = 1; i < openSet.Count; i++){
                // compare the fCost, if the fcost is the same then compare the hCost
                // set the currentNode to the lowest cost node
                if (openSet[i].fCost < currentNode.fCost  || 
                        (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)){
                    currentNode = openSet[i];
                }
            }
            // remove the newly selected node from the open list
            openSet.Remove(currentNode);
            // add the selected node to the closed list
            closedSet.Add(currentNode);
            // if the selected node is the target node then retrace the parents of each node
            // in the closed list iteratively from the targetNode
            if (currentNode == targetNode){
                RetracePath(startNode, targetNode);
                return;
            }
            // else find the neighbour nodes to the newly selected node
            // loop through each of them adding them to the open list
            foreach (Astar2DNode neighbour in grid.GetNeighbours(currentNode)){
                // if the neighbouring node is not a walkable node or is in the closed list
                // do not add it to the open list
                if (!neighbour.walkable || closedSet.Contains(neighbour)){
                    continue;
                }
                // calculate the gCost of the selected node
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                // if the new gCost of neighbours is less than the current neighbour cost of neighbour
                // or it is not yet in the open list, replace the neighbour or add it to the list
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)){
                    // calculate the gCost
                    neighbour.gCost = newMovementCostToNeighbour;
                    // calculate the hCost
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    // ^ this automatically updates the fCost as fCost = gCost + hCost

                    // then we set the "parent" of the neighbour to the currentNode
                    neighbour.parent = currentNode;
                    // if the open list doesn't contain neighbour add it to the list
                    if (!openSet.Contains(neighbour)){
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }
    // calculate the distance between two nodes
    int GetDistance(Astar2DNode nodeA, Astar2DNode nodeB){
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        // 14 is sqrt(2) * 10, 1 is 1 * 10
        if (distanceX > distanceY)
        {
            return (14 * distanceY + 10 * (distanceX - distanceY));
        }
        return (14 * distanceX + 10 * (distanceY - distanceX));
    }
    // using the parents of each node we retrace the path to the startNode
    void RetracePath(Astar2DNode startNode, Astar2DNode endNode){
        List<Astar2DNode> path = new List<Astar2DNode>();
        Astar2DNode currentNode = endNode;
        // loop the Nodes from the currentNode until we reach the startNode
        while (currentNode != startNode){
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        // since we traced from the endNode we must reverse the list
        path.Reverse();
        // set the path
        grid.path = path;
    }
}
