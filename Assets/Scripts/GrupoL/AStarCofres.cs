using System.Collections.Generic;
using System.Linq;
using Navigation.Interfaces;
using Navigation.World;
using Unity.VisualScripting;
using UnityEngine;

//ALGORITMO A* PARA LOS COFRES Y LA SALIDA

namespace GrupoL {

    //Iris Muñoz, 17/11/2025

    public class AStarCofres : INavigationAlgorithm
    {

        private WorldInfo _world;
        
        public void Initialize(WorldInfo world)
        {
            _world = world;
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
