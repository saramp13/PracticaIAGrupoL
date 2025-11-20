using System.Collections.Generic;
using Navigation.Interfaces;
using Navigation.World;
using UnityEngine;


namespace GrupoL
{
    // Implementación de la interfaz INavigationAlgorithm

    // Sara Mesa y Claudia Morago, 2/11/2025
    /* Esta clase implementa el aldoritmo A*, es decir, implementa la búsqueda del camino
     * óptimo (el más corto desde un punto inicial al objetivo). 
     * Esta clase crea una representación del mapa con la clase Nodo. Se crea un camino óptimo
     * usando dos listas distintas de nodos pendientes y ya procesados eligiendo siempre el 
     * fCost menor y se exploran los vecinos caminables de cada nodo. Si es necesario
     * actualiza los costes hasta tener el mejor camino. Por último reconstruye el camino final
     * devolviendo una lista de celdas. 
     * 
     * Esta clase hace uso de WorldInfo como base de datos del escenario. 
     * Esta clase hace uso de CellInfo para acceder a coordenadas, saber tipos de celdas,
     * si es caminable o la posición en el mundo 3D entre otras.
     */

    public class AStar : INavigationAlgorithm
    {
        private WorldInfo _world; //Referencia al mundo
        private Node[,] _nodeGrid; //Array de nodos único por celda

        public void Initialize(WorldInfo world)
        {
            _world = world;

            int sizeX = _world.WorldSize.x;
            int sizeY = _world.WorldSize.y;
            _nodeGrid = new Node[sizeX, sizeY];

            for (int x = 0; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                    _nodeGrid[x, y] = new Node(_world[x, y]);
        }

        public CellInfo[] GetPath(CellInfo start, CellInfo goal)
        {

            //Convertimos las CellInfo en Nodes para poder asignar costes
            Node startNode = _nodeGrid[start.x, start.y];
            Node goalNode = _nodeGrid[goal.x, goal.y];

            // Reiniciar todos los nodos antes de la búsqueda
            foreach (var node in _nodeGrid)
            {
                node.gCost = int.MaxValue;
                node.hCost = 0;
                node.parent = null;
            }

            List<Node> openSet = new List<Node>(); // Nodos por evaluar
            List<Node> closeSet = new List<Node>();// Nodos evaluados
            openSet.Add(startNode);

            startNode.gCost = 0;
            startNode.hCost = Heuristic(startNode.cell, goalNode.cell);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                // Queremos que el nodo actual = nodo abierto con el menor cost f posible
                for (int i = 1; i < openSet.Count; i++)
                {
                    if ((openSet[i].fCost < currentNode.fCost) || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    {
                        currentNode = openSet[i];
                    }
                }
                //Quitar el nodo evaluado de open y añadir a close
                openSet.Remove(currentNode);
                closeSet.Add(currentNode);

                //Si se encuentra el nodo meta
                if (currentNode.cell == goalNode.cell)
                {
                    return ReconstructPath(startNode, currentNode);
                }

                //Evaluación de nodos vecinos
                foreach (var neighbourCell in GetNeighbours(currentNode.cell))
                {
                    if (!neighbourCell.Walkable) { continue; } //Saltamos los nodos que no podemos pisar

                    Node neighbour = _nodeGrid[neighbourCell.x, neighbourCell.y];

                    if (closeSet.Contains(neighbour)) { continue; }//Saltamos los nodos que ya se habían analizado

                    //Costo del nuevo nodo
                    int newCost = currentNode.gCost + Heuristic(currentNode.cell, neighbour.cell);
                    //Evaluación del costo y selección del menor
                    if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCost;
                        neighbour.hCost = Heuristic(neighbour.cell, goalNode.cell);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }

                }
            }

            //Si no hay información para hacer el camino 
            if (_world == null || _nodeGrid == null) return new CellInfo[0];
            return new CellInfo[0];

        }

        //Clase busqueda vecinos (4 celdas, drch,izq, up, down)
        private IEnumerable<CellInfo> GetNeighbours(CellInfo cell)
        {
            int x = cell.x;
            int y = cell.y;

            //Si existe una celda a la izq.
            if (x > 0) yield return _world[x - 1, y];
            //Si existe una celda a la drch.
            if (x < _world.WorldSize.x - 1) yield return _world[x + 1, y];
            //Si existe una celda arriba
            if (y > 0) yield return _world[x, y - 1];
            //Si existe una celda abajo
            if (y < _world.WorldSize.y - 1) yield return _world[x, y + 1];
        }

        private int Heuristic(CellInfo a, CellInfo b)
        {
            // Manhattan (4 direcciones)
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        //Clase que devuelve el camino en orden
        private CellInfo[] ReconstructPath(Node start, Node end)
        {
            List<CellInfo> path = new List<CellInfo>();
            Node current = end;
            while (current != start)
            {
                path.Add(current.cell);
                current = current.parent;
                if (current == null) break;
            }
            path.Reverse();
            return path.ToArray();
        }


    }
}
