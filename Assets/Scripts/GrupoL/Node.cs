using UnityEngine;
using System.Collections;

public class Node
{
    public bool walkable;
    public Vector3 worldPos;//posición del nodo en el mundo
    public int gridX; // posición del nodo en el grid en x
    public int gridY;  // posición del nodo en el grid en y

    public int gCost;
    public int hCost;
    public Node parent;


    public Node(bool _walkable, Vector3 _worldPos,int _gridX, int _gridY ){
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;

    }
    
    public int fCost
    {
        get
        {
            return gCost+hCost;
        }
    }
}
