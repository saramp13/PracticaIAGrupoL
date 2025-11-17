using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarManager : MonoBehaviour
{
    public Transform seeker,target;

    Grid grid;

    [SerializeField] private AgentMovement agentMove;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        FindPath(seeker.position, target.position);
    }


    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //Pasar posiciones nodo a posiciones en mundo (x,y)
        Node startNode = grid.NodeinGameWorld(startPos);
        Node targetNode = grid.NodeinGameWorld(targetPos);

        List<Node> openSet = new List<Node>(); // nodos por evaluar
        List<Node> closeSet = new List<Node>();// nodos evaluados
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            // el nodo actual = nodo abierto con el menor cost f posible
            for (int i = 1; i < openSet.Count; i++)
            {
                if ((openSet[i].fCost < currentNode.fCost) || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            //quitar el nodo de open y añadir a close
            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            //si se ha encontrado el nodo meta
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach(Node neighbour in grid.GetNeighbours(currentNode))
            {
                if(!neighbour.walkable || closeSet.Contains(neighbour)){
                    continue; //saltamos los nodos que no podemos pisar o que ya se han analizado
                }

                //si el nuevo camino al vecino es más corto o no esta en la lista open
                int newMoveCostToNeighbour = currentNode.gCost + GetDistance(currentNode,neighbour);
                if(newMoveCostToNeighbour < neighbour.gCost|| !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMoveCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour,targetNode);
                    neighbour.parent= currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
            



        }
    }

    void RetracePath(Node startNode,Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path=path;

        //Movimiento del personaje, llamamos a SearchAgent
        if (agentMove != null)
        {
            agentMove.SetPath(grid.path);
        }

    }

    //Distancia entre dos nodos teniendo en cuenta diagonales
    int GetDistance(Node node1, Node node2)
    {
        int distanceX = Mathf.Abs(node1.gridX - node2.gridX);
        int distanceY = Mathf.Abs(node1.gridY - node2.gridY);
        if( distanceX > distanceY)
        {
            return (14 * distanceY + 10*(distanceX - distanceY));
        }
        return (14 * distanceX + 10*(distanceY - distanceX));
    }
}







