using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SearchAgent: MonoBehaviour
{
    [SerializeField] private Transform agent;   // Le pasamos la referencia del personaje
    private List<Node> path;                   // Camino que le pasa el A*
    private int currentIndex = 0;
    public float speed = 5f;

    // Este método se llama desde tu AStarManager después de generar el camino
    public void SetPath(List<Node> newPath)
    {
        if (newPath == null || newPath.Count == 0) return;
        path = newPath;
        currentIndex = 0;
    }

    void Update()
    {
        if (path == null || path.Count == 0) return; // No hay camino, nada que hacer

        // Obtenemos el nodo objetivo actual
        Vector3 targetPosition = path[currentIndex].worldPos;

        // Movemos el agente hacia el nodo
        agent.position = Vector3.MoveTowards(agent.position, targetPosition, speed * Time.deltaTime);

        // Si llegamos al nodo actual, pasamos al siguiente
        if (Vector3.Distance(agent.position, targetPosition) < 0.1f)
        {
            currentIndex++;

            // Si llegamos al final del camino
            if (currentIndex >= path.Count)
            {
                path = null; // Se acabó el movimiento
            }
        }
    }
}







