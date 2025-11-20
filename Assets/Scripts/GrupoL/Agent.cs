using System.Collections.Generic;
using Navigation.Interfaces;
using Navigation.World;
using UnityEngine;


namespace GrupoL
{
    //Implementacion de la interfaz INavigationAgent

    //Claudia Morago, 8/11/2025

    /* Esta clase gestiona el camino que debe seguir el agente, usando los metodos de la interfaz INavigationAgent
    *  El camino mas optimo se calcula usando el algoritmo A* en la clase AStar
    *  Esta clase Agent obtiene el camino de celdas calculado por AStar y forma una cola ordenada,
    *  que luego utiliza para ir indicando la siguiente posicion
    *  La logica del movimiento la gestiona NavigationMovement
    *  Esta clase Agent se comunica con NavigationMovement mediante la interfaz INavigationAgent, indic�ndole cuales
    *  son las coordenadas de la siguiente posici�n para que el personaje se mueva a esa posici�n
    */

    public class Agent : INavigationAgent
    {
        public CellInfo CurrentObjective { get; private set; }  //celda final (salida)
        public Vector3 CurrentDestination { get; private set; } //celda siguiente
        public int NumberOfDestinations { get; private set; }   //contador de celdas que hay que recorrer

        private WorldInfo _world;                               //referencia al WorldInfo              
        private INavigationAlgorithm _algorithm;                //referencia al algoritmo A* 
        private Queue<CellInfo> _path;                          //cola con el camino final calculado

        // Este metodo inicializa el agente y establece Exit como objetivo 
        public void Initialize(WorldInfo world, INavigationAlgorithm navigationAlgorithm)
        {
            _world = world;
            _algorithm = navigationAlgorithm;
            _algorithm.Initialize(world);

            CurrentObjective = _world.Exit;
            NumberOfDestinations = 1;
        }

        // Este metodo devuelve la siguiente posici�n (celda) a la que el agente debe moverse 
        // Si no hay calculada una ruta previa o ya se ha recorrido, se genera una nueva llamando a GetPath() del algoritmo AStar
        // Adem�s se comprueba que existe un camino v�lido hasta la salida (no hay mundo bloqueado)
        public Vector3? GetNextDestination(Vector3 currentPosition)
        {
            // Comprobamos si no tenemos ruta o ya se ha recorrido 
            if (_path == null || _path.Count == 0)
            {
                var currentCell = _world.FromVector3(currentPosition);
                var result = _algorithm.GetPath(currentCell, CurrentObjective); // Usa GetPath para obtener la ruta que ha calculado A*

                ////Si se quiere simular el supuesto de que no hay camino (no hay lista) usar:
                //var result = new CellInfo[0];

                _path = new Queue<CellInfo>(result);    //Se ordenan las celdas en una cola

                // Comprueba que hay un camino posible, si no devuelve null y un mensaje
                if (_path.Count == 0)
                {
                    Debug.LogWarning($"No existe ningun camino posible al destino. Mundo bloqueado");
                    return null;
                }
            }

            var next = _path.Dequeue();     //Va obteniendo las celdas de la cola        
            CurrentDestination = _world.ToWorldPosition(next);  // Se pasan la celda a coordenadas de mundo
            return CurrentDestination;  
        }

    }
}