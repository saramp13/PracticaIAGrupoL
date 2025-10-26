using UnityEngine;
using System.Collections;
using System;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;
    public Transform player;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius*2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);//Cuantos nodos hay en  dirección X
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);//Cuantos nodos hay en dirección Y
        CreateGrid(); //Genera los nodos
    
    }


    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 wBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;// Devuelve la esquina inferior izquierda (se empezara a poner nodos desde ahí)


        //Se comprueba cuales de los nodos son walkable recorriendo el grid
        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //Calcular los puntos donde existen nodos en el grid
                //Empezamos en wBottomLeft, sumamos X (right) y Z (forward) y con nodeRadius centramos cada nodo en su celda
                Vector3 worldPoint = wBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //Creamos una esfera alrededor de nodeRadius de cada noda y si no colisiona con nada es walkable
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius,unwalkableMask));
                //Guarda los nodos con su información
                grid[x,y] = new Node(walkable, worldPoint);
            }
        }
    }

    //Convertir posiciones (x,y) del escenario en un Nodo
    Node NodeinGameWorld(Vector3 worldPos)
    {
        //Convertimos en porcentaje la posición de nodo
        float percentX = (worldPos.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x; //Dónde cae la posición dentro del grid como porcentaje
        float percentY = (worldPos.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y;
        //Para que no se salga del grid establecido clamp
        percentX= Mathf.Clamp01(percentX);
        percentY=Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];

    }



    void OnDrawGizmos() //Wireframe del grid en unity
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        //comprobación de creación grid nodos con colores
        //if (grid != null)
        //{
        //    Node playerNode = NodeinGameWorld(player.position);
        //    foreach (Node n in grid)
        //    {
        //        Gizmos.color = (n.walkable) ? Color.white : Color.red;
        //        //comprobación función de nodo a coordenadas en el agente
        //        if (playerNode == n)
        //        {
        //            Gizmos.color = Color.cyan;
        //        }
        //        Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - .1f));

        //    }
        //}
    }
}
