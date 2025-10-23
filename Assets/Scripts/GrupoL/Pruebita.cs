using UnityEngine;
using Navigation.Interfaces;
using Navigation.World;
using System.Collections.Generic;
using System.Collections.Specialized;

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
            Vector3 startPos = new Vector3(startNode.x, 0, startNode.y);
            Vector3 targetPos = new Vector3(targetNode.x, 0, targetNode.y);
            startRecord.H = Vector3.Distance(startPos,targetPos);

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
                toSearch.Remove(current); //Se quita de la lista de por explorar
          
                //Si el nodo current es el mismo que el target
                if(current.Cell == targetNode)
                {
                    
                }
            
            }

            return new CellInfo[0];

            //throw new System.NotImplementedException();
        }
    }

    
}
