using UnityEngine;
using System.Collections;

public class Node
{
    public bool walkable;
    public Vector3 worldPos;//posición del nodo en el mundo

    public Node(bool _walkable, Vector3 _worldPos ){
        walkable = _walkable;
        worldPos = _worldPos;

    }
}
