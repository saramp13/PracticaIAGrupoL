using System.Collections.Generic;
using Navigation.Interfaces;
using Navigation.World;
using UnityEngine;

//ESQUELETO CODIGO BUENO: IMPLEMENTAR EL ALGORITMO A*

namespace GrupoL {
    public class AStar : INavigationAlgorithm
    {

        private WorldInfo _world;

        public void Initialize(WorldInfo world)
        {
            _world = world;
        }

        public CellInfo[] GetPath(CellInfo start, CellInfo goal)
        {
            var openSet = new List<CellInfo> { start };
            var closedSet = new HashSet<CellInfo>();

            var cameFrom = new Dictionary<CellInfo, CellInfo>();
            var gCost = new Dictionary<CellInfo, int> { [start] = 0 };
            var fCost = new Dictionary<CellInfo, int> { [start] = Heuristic(start, goal) };

            while (openSet.Count > 0)
            {
                // Nodo con menor fCost
                CellInfo current = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (fCost[openSet[i]] < fCost[current])
                        current = openSet[i];
                }

                if (current == goal)
                    return ReconstructPath(cameFrom, current);

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var neighbour in GetNeighbours(current))
                {
                    if (closedSet.Contains(neighbour) || !neighbour.Walkable)
                        continue;

                    int tentativeG = gCost[current] + 1;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else if (tentativeG >= gCost.GetValueOrDefault(neighbour, int.MaxValue))
                        continue;

                    cameFrom[neighbour] = current;
                    gCost[neighbour] = tentativeG;
                    fCost[neighbour] = tentativeG + Heuristic(neighbour, goal);
                }
            }

            // No hay camino
            return new CellInfo[0];
        }

        //TECNICAMENTE ESTO SE PODRIA PONER EN WORLDINFO PERO ME DA MIEDITO TOCAR ESA CLASE
        // SI LO HICIERAMOS ASI SERIA REUTILIZABLE PARA OTROS ALGORITMOS
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

        private CellInfo[] ReconstructPath(Dictionary<CellInfo, CellInfo> cameFrom, CellInfo current)
        {
            var path = new List<CellInfo> { current };
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
