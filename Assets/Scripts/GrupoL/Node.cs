using System.Collections.Generic;
using Navigation.Interfaces;
using Navigation.World;
using UnityEngine;

namespace GrupoL
{
    // Clase Nodo que envuelve CellInfo

    // Sara Mesa, 21/11/2025
    /*
     * En esta clase se crean nodos que contienen toda la información de la celda (CellInfo)
     * y se añaden variables adicionales para el cálculo de costes y reconstrucción del camino
     * con los nodos padre.
     * Esta clase sirve como apoyo al algoritmo A*, ya que describe el estado interno.
     * Esta clase evita modificar datos del mapa (WorldInfo y por tanto CellInfo). 
     * Cada celda del mapa tiene asociado un único nodo, evitando crear objetos nuevos
     * durante cada ejecución del algoritmo.
     * El propósito principal es organizativo y permite redicit el uso de memoria, así como
     * simplifica la implementación del algoritmo A*.
     */
    public class Node
    {
        public CellInfo cell;
        public int gCost;
        public int hCost;
        public Node parent;

        public Node(CellInfo cell)
        {
            this.cell = cell;
            gCost = int.MaxValue;
            hCost = 0;
            parent = null;
        }

        public int fCost => gCost + hCost;
    }
}

