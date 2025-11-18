using System.Collections.Generic;
using Navigation.Interfaces;
using Navigation.World;
using UnityEngine;

//ESQUELETO CODIGO BUENO: IMPLEMENTAR EL MOVIMIENTO DEL AGENTE
// SE SUPONE FUNCIONA

namespace GrupoL {
    public class Agent : INavigationAgent
    {
        public CellInfo CurrentObjective { get; private set; }
        public Vector3 CurrentDestination { get; private set; }
        public int NumberOfDestinations { get; private set; }

        private WorldInfo _world;
        private INavigationAlgorithm _algorithm;
        private Queue<CellInfo> _path;

        public void Initialize(WorldInfo worldInfo, INavigationAlgorithm navigationAlgorithm)
        {
            _world = worldInfo;
            _algorithm = navigationAlgorithm;

            _algorithm.Initialize(worldInfo);

            // Objetivo por defecto: la salida del mundo
            CurrentObjective = _world.Exit;

            NumberOfDestinations = 1;
        }

        public Vector3? GetNextDestination(Vector3 currentPosition)
        {
            // Si no hay camino o ya se consumió:
            if (_path == null || _path.Count == 0)
            {
                var startCell = _world.FromVector3(currentPosition);

                // Le pedimos al algoritmo UNA RUTA COMPLETA
                var result = _algorithm.GetPath(startCell, CurrentObjective);

                if (result == null || result.Length == 0)
                    return null;

                _path = new Queue<CellInfo>(result);
            }

            // Consumimos el siguiente waypoint
            var nextCell = _path.Dequeue();
            CurrentDestination = _world.ToWorldPosition(nextCell);
            return CurrentDestination;
        }
    }
}
