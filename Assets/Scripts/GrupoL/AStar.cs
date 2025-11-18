using System.Collections.Generic;
using Navigation.Interfaces;
using Navigation.World;
using UnityEngine;
using static Navigation.World.CellInfo;

//ESQUELETO CODIGO BUENO: IMPLEMENTAR EL ALGORITMO A*
// HAY QUE MIRAR LOS ERRORES

namespace GrupoL {
    public class AStar : INavigationAlgorithm
    {
        private WorldInfo _world;

        public void Initialize(WorldInfo worldInfo)
        {
            _world = worldInfo;
        }

        public CellInfo[] GetPath(CellInfo startNode, CellInfo goalNode)
        {
            // OPEN y CLOSED
            var openSet = new List<CellInfo>();
            var closedSet = new HashSet<CellInfo>();

            openSet.Add(startNode);

            // Diccionarios para almacenar costes y padres
            var gCost = new Dictionary<CellInfo, int>();
            var fCost = new Dictionary<CellInfo, int>();
            var cameFrom = new Dictionary<CellInfo, CellInfo>();

            gCost[startNode] = 0;
            fCost[startNode] = Heuristic(startNode, goalNode);

            while (openSet.Count > 0)
            {
                // Seleccionamos el nodo con menor fCost
                CellInfo current = openSet[0];
                foreach (var node in openSet)
                {
                    if (fCost[node] < fCost[current])
                        current = node;
                }

                // Si hemos llegado al objetivo
                if (current == goalNode)
                    return ReconstructPath(cameFrom, current);

                openSet.Remove(current);
                closedSet.Add(current);

                // Vecinos
                foreach (var neighbour in _world.GetNeighbours(current))
                {
                    // Saltamos muros, límites y celdas bloqueadas
                    if (neighbour.Type == CellType.Wall || neighbour.Type == CellType.Limit)
                        continue;

                    if (closedSet.Contains(neighbour))
                        continue;

                    int tentative = gCost[current] + 1; // coste uniforme (4 direcciones)

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else if (tentative >= gCost[neighbour])
                        continue;

                    cameFrom[neighbour] = current;
                    gCost[neighbour] = tentative;
                    fCost[neighbour] = tentative + Heuristic(neighbour, goalNode);
                }
            }

            return new CellInfo[0]; // Si no hay camino
        }

        private int Heuristic(CellInfo a, CellInfo b)
        {
            // Manhattan (recomendada para movimiento en 4 direcciones)
            return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
        }

        private CellInfo[] ReconstructPath(Dictionary<CellInfo, CellInfo> cameFrom, CellInfo current)
        {
            var path = new List<CellInfo>();
            path.Add(current);

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }

            path.Reverse();
            return path.ToArray();
        }
    }
}
