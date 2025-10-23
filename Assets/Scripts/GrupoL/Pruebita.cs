using UnityEngine;
using Navigation.Interfaces;
using Navigation.World;
using System.Collections.Generic;

public class Pruebita
{
    public class AlgoritmoA: INavigationAlgorithm
    {
        private WorldInfo _world;

        public void Initialize(WorldInfo worldInfo)
        {
            _world= worldInfo;
        }

        public CellInfo[] GetPath(CellInfo startNode, CellInfo targetNode)
        {
            var startRecord = new NodeRecord(startNode);
            startRecord.G = 0;
            startRecord.H = Vector3.Distance(startNode.Position, targetNode.Position);

            //Creamos dos listas para almacenar nodos
            var toSearch = new List<NodeRecord>() { startRecord }; //Nodos por explorar empezando por el inicial
            var processed = new List<NodeRecord>(); //Nodos procesados (se usa para no repetir los ya visitados)

            //---Implementación Algoritmo de busqueda A*---

            //Mientras que haya nodos que explorar o haya un camino:
            while (toSearch.Count > 0)
            {
                NodeRecord current = toSearch[0];
                foreach(var t in toSearch)
                {
                    // Si la F del nodo siguiente es menor que la del nodo actual o igual pero su H es menor:
                    if(t.F < current.F || (t.F == current.F && t.H < current.H))
                        current = t;
                }

                processed.Add(current); //Añadimos a la lista el nodo final
                toSearch.RemoveAt(current); //Se quita de la lista de por explorar
          
                //Si el nodo current es el mismo que el target
                if(current == targetNode)
                {
                    
                }
            
            }

            




            //throw new System.NotImplementedException();
        }
    }

    
}
