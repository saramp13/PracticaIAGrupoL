using System.Collections.Generic;
using System.Linq;
using Navigation.Interfaces;
using Navigation.World;
using Unity.VisualScripting;
using UnityEngine;

//ALGORITMO A* PARA LOS COFRES Y LA SALIDA

namespace GrupoL {

    //Iris Muñoz, 15/11/2025
    /*
     Esta clase modifica el método GetPath() para que el agente se mueva hacia los cofres y los visite del más cercano al más lejano.
     Se recorre el grid en busca de la posición de los cofres, se calcula la distancia hasta ellos desde el agente y se introducen
     en una lista en orden del menor al mayor coste.
     Se calcula un recorrido completo con todos los cofres y la salida para que el agente lo siga.
     */

    public class AStarCofres : INavigationAlgorithm
    {

        private WorldInfo _world;
        private Node[,] _nodeGrid;

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
            List<CellInfo> Treasures = new List<CellInfo>(); //Creamos la lista con todos los cofres de la escena

            for (int x = 0; x < _world.WorldSize.x; x++) //Miramos en qué casillas están los cofres y las añadimos a la lista
                for (int y = 0; y < _world.WorldSize.y; y++)
                    if (_world[x, y].Type == CellInfo.CellType.Treasure)
                        Treasures.Add(_world[x, y]);

            List<CellInfo> RecorridoCompleto = new List<CellInfo>(); //Lista que va a contener tanto los cofres como la salida para ir en orden
            CellInfo current = start;

            while (Treasures.Count > 0) //Mientras queden cofres por recoger 
            {
                CellInfo masCercano = null; //Almacena la posición del cofre más cercano al agente
                CellInfo[] camino = null; //Almacena el camino calculado con A* hasta dicho cofre

                foreach (var c in Treasures) //Calcula las rutas a todos los cofres restantes desde la posición actual y actualiza el cofre a seguir
                {
                    var path = AStar(current, c);
                    if (path.Length > 0 && (camino == null || path.Length < camino.Length))
                    {
                        camino = path;
                        masCercano = c;
                    }
                }

                if (camino == null) //Si hay algún cofre inaccesible sale del bucle e imprime un mensaje en la consola
                {
                    Debug.Log("Encontrado cofre inaccesible, recorrido terminado.");
                    break;
                }

                RecorridoCompleto.AddRange(camino.Skip(1)); //Salta la celda actual pues está ya dentro de current
                current = masCercano; //Actualiza el objetivo del agente 
                Treasures.Remove(masCercano); //Borra el cofre recogido de la lista
            }

            var pathSalida = AStar(current, goal); //Una vez recogidos todos los cofres calculamos el camino hacia la salida también con A*
            RecorridoCompleto.AddRange(pathSalida.Skip(1)); //Añadimos el recorrido hacia la salida al recorrido completo

            return RecorridoCompleto.ToArray();
        }

        private CellInfo[] AStar(CellInfo start, CellInfo goal) //Vamos a usar el algoritmo A* secuencialmente, por eso lo separamos en otro método
        {
            Node startNode = _nodeGrid[start.x, start.y];
            Node goalNode = _nodeGrid[goal.x, goal.y];
            // Reset de nodos
            foreach (var node in _nodeGrid)
            {
                node.gCost = int.MaxValue;
                node.hCost = 0;
                node.parent = null;
            }

            List<Node> openSet = new List<Node>();
            List<Node> closedSet = new List<Node>();
            openSet.Add(startNode);
            startNode.gCost = 0;
            startNode.hCost = Heuristic(start, goal);

            while (openSet.Count > 0)
            {
                Node current = openSet[0];

                // Nodo con menor fCost
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < current.fCost ||
                        (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                        current = openSet[i];
                }
                openSet.Remove(current);
                closedSet.Add(current);

                if (current == goalNode)
                    return ReconstructPath(startNode, current);

                foreach (var neighbourCell in GetNeighbours(current.cell))
                {
                    if (!neighbourCell.Walkable)
                        continue;

                    Node neighbour = _nodeGrid[neighbourCell.x, neighbourCell.y];

                    if (closedSet.Contains(neighbour))
                        continue;

                    int tentativeG = current.gCost + 1;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else if (tentativeG >= neighbour.gCost)
                        continue;

                    neighbour.gCost = tentativeG;
                    neighbour.hCost = Heuristic(neighbour.cell, goalNode.cell);
                    neighbour.parent = current;
                }
            }

            // No hay camino
            return new CellInfo[0];
        }

        private IEnumerable<CellInfo> GetNeighbours(CellInfo cell)
        {
            int x = cell.x;
            int y = cell.y;

            if (x > 0) yield return _world[x - 1, y];
            if (x < _world.WorldSize.x - 1) yield return _world[x + 1, y];
            if (y > 0) yield return _world[x, y - 1];
            if (y < _world.WorldSize.y - 1) yield return _world[x, y + 1];
        }

        private int Heuristic(CellInfo a, CellInfo b)
        {
            // Manhattan (4 direcciones)
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

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
