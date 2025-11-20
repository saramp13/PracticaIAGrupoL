using System.Collections.Generic;
using Navigation.Interfaces;
using Navigation.World;
using UnityEngine;

//IMPLEMENTACIÓN DE LA BÚSQUEDA ONLINE DE ZOMBIES

namespace GrupoL {

    //Iris Muñoz 18/11/2025

    public class AgentZombies : INavigationAgent
    {
        public CellInfo CurrentObjective { get; private set; }  //celda final (salida)
        public Vector3 CurrentDestination { get; private set; } //celda siguiente
        public int NumberOfDestinations { get; private set; }   //contador de celdas que hay que recorrer

        private WorldInfo _world;                               //referencia al WorldInfo              
        private INavigationAlgorithm _algorithm;                //referencia al algoritmo A* 
        private Queue<CellInfo> _path;                          //cola con el camino final calculado


        public void Initialize(WorldInfo world, INavigationAlgorithm navigationAlgorithm)
        {
            _world = world;
            _algorithm = navigationAlgorithm;
            _algorithm.Initialize(world);

            // Se selecciona el objetivo
            CurrentObjective = _world.Exit;
            NumberOfDestinations = 1;
        }

        //DETERMINAMOS SI HAY ALGÚN ZOMBIE CERCA DEL AGENTE
        private CellInfo ZombieMasCercano(CellInfo current)
        {
            CellInfo zombieObjetivo = null; //Almacena el zombie al que se perseguiría
            int mejorDist = int.MaxValue; //Almacena la distancia hasta el zombie más cercano

            for (int x = 0; x < _world.WorldSize.x; x++) //Se recorren las celdas del grid para buscar a los zombies
            {
                for (int y = 0; y < _world.WorldSize.y; y++)
                {
                    var cell = _world[x, y]; 

                    if (cell.Type != CellInfo.CellType.Enemy) //Si no se encuentra un zombie se repite el bucle
                        continue;

                    int dist = Mathf.Abs(cell.x - current.x) + Mathf.Abs(cell.y - current.y); //Se calcula la distancia hasta el zombie encontrado

                    if (dist < mejorDist) //Se asigna como objetivo el zombie más cerca del agente
                    {
                            mejorDist = dist;
                            zombieObjetivo = cell;
                    }
                }
            }

            return zombieObjetivo;
        }


        //SI NO HAY ZOMBIES CERCA SE BUSCAN LOS COFRES
        private CellInfo CofreMasCercano(CellInfo current)
        {
            CellInfo mejorCofre = null; //Almacena el cofre más cerca del agente
            int mejorDist = int.MaxValue; //Almacena la distancia hasta el cofre más cercano

            for (int x = 0; x < _world.WorldSize.x; x++) //Se recorren las celdas del grid en busca de cofres
            {
                for (int y = 0; y < _world.WorldSize.y; y++)
                {
                    var cell = _world[x, y];

                    if (cell.Type != CellInfo.CellType.Treasure) //Si no quedan cofres se buscaría la salida
                        continue;

                    var camino = _algorithm.GetPath(current, cell); //Se guarda el camino hasta el cofre objetivo

                    if (camino != null && camino.Length > 0 && camino.Length < mejorDist) //Si el camino es accesible y es el cofre más cerca 
                    {                                                                     //se señala como objetivo final
                        mejorDist = camino.Length;
                        mejorCofre = cell;
                    }
                }
            }

            return mejorCofre;
        }

        //ESTOS MÉTODOS DEVUELVEN LAS DISTANCIAS NUMÉRICAS AL ZOMBIE Y COFRE MÁS CERCANO PARA COMPARARLOS ENTRE SÍ
        //Y perseguir entonces al que esté más cerca
        private int DistNumAlZombie(CellInfo current)
        {
            var zombie = ZombieMasCercano(current);
            if (zombie == null)
                return int.MaxValue;

            var path = _algorithm.GetPath(current, zombie);
            if (path == null || path.Length == 0)
                return int.MaxValue; //Zombie inaccesible

            return path.Length;
        }

        private int DistNumAlCofre(CellInfo current)
        {
            var cofre = CofreMasCercano(current);
            if (cofre == null)
                return int.MaxValue;

            var path = _algorithm.GetPath(current, cofre);
            if (path == null || path.Length == 0)
                return int.MaxValue; //Cofre inaccesible

            return path.Length;
        }

        public Vector3? GetNextDestination(Vector3 currentPosition)
        {
            var currentCell = _world.FromVector3(currentPosition);
            var objetivoAnterior = CurrentObjective; //Almacena el objetivo anterior al actual

            //Se trata de buscar primero si hay zombies cerca (prioritario dado que se mueven)
            CellInfo zombie = ZombieMasCercano(currentCell);
            CellInfo cofre = CofreMasCercano(currentCell);
            int distZombie = DistNumAlZombie(currentCell); //Almacena la distancia al zombie más cercano para comparar
            int distCofre = DistNumAlCofre(currentCell); //Almacena la distancia al cofre más cercano para comparar

            if (zombie != null && (distZombie<=distCofre))
            {
                CurrentObjective = zombie;
            }
            else if (cofre != null)
            {
                CurrentObjective = cofre;
            }
            else
            {
                CurrentObjective = _world.Exit;
            }

            //Si el camino está vacío o si el objetivo cambia se recalcula
            if (_path == null || _path.Count == 0 || objetivoAnterior != CurrentObjective)
            {
                var result = _algorithm.GetPath(currentCell, CurrentObjective); //Almacena el siguiente camino a seguir

                ////Si se quiere simular el supuesto de que no hay camino (no hay lista) usar:
                //var result = new CellInfo[0];

                if (result == null || result.Length == 0) 
                {
                    Debug.LogWarning($"No existe ningun camino posible al destino. Mundo bloqueado");
                    return null;
                }
                    

                _path = new Queue<CellInfo>(result);
            }

            var next = _path.Dequeue();
            CurrentDestination = _world.ToWorldPosition(next);

            return CurrentDestination;
        }


    }
}
