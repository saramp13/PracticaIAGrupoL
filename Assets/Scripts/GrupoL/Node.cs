using System.Collections.Generic;
using Navigation.Interfaces;
using Navigation.World;
using UnityEngine;

namespace GrupoL
{
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

