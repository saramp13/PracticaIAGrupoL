using System.Collections.Generic;
using System.Linq;
using Navigation.Interfaces;
using Navigation.World;
using Unity.VisualScripting;
using UnityEngine;

//ALGORITMO A* QUE IMPLEMENTA LA CAPTURA DE ZOMBIES

namespace GrupoL {

    //Iris Muñoz, 18/11/2025

    public class AStarZombies : INavigationAlgorithm
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

        //PARA LA BÚSQUEDA ONLINE IMPLEMENTAMOS EL MÉTODO GETPATH SIN PRIORIDADES, PUES ESO SERÁ TAREA DEL AGENTE EN AGENT
        public CellInfo[] GetPath(CellInfo start, CellInfo goal)
        {
            return AStar(start, goal);
        }

        private CellInfo[] AStar(CellInfo start, CellInfo goal) 
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
