using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public List<Waypoint> CalcularRuta(
        Waypoint inicio,
        Waypoint objetivo)
    {
        // Nodos que tenemos pendientes de revisar
        List<Waypoint> abiertos = new List<Waypoint>();

        // Nodos que ya hemos revisado
        HashSet<Waypoint> cerrados = new HashSet<Waypoint>();

        // Para reconstruir el camino al final
        Dictionary<Waypoint, Waypoint> padres =
            new Dictionary<Waypoint, Waypoint>();

        // Coste desde el inicio hasta cada nodo
        Dictionary<Waypoint, float> costeG =
            new Dictionary<Waypoint, float>();

        // Coste total estimado: F = G + H
        Dictionary<Waypoint, float> costeF =
            new Dictionary<Waypoint, float>();

        // Empezamos en el waypoint inicial
        abiertos.Add(inicio);
        costeG[inicio] = 0;
        costeF[inicio] = CalcularHeuristica(inicio, objetivo);

        while (abiertos.Count > 0)
        {
            // Cogemos el nodo con menor F
            Waypoint actual = ObtenerMenorF(
                abiertos,
                costeF
            );

            // Hemos llegado al objetivo
            if (actual == objetivo)
            {
                return ReconstruirRuta(
                    padres,
                    actual
                );
            }

            // Movemos el nodo de abiertos a cerrados
            abiertos.Remove(actual);
            cerrados.Add(actual);

            // Revisamos todos los vecinos
            foreach (Waypoint vecino in actual.vecinos)
            {
                if (vecino == null)
                    continue;

                if (cerrados.Contains(vecino))
                    continue;

                // Coste de ir de actual a vecino
                float distancia = Vector3.Distance(
                    actual.transform.position,
                    vecino.transform.position
                );

                float nuevoCosteG =
                    costeG[actual] + distancia;

                // Si todavía no conocemos este nodo
                if (!costeG.ContainsKey(vecino))
                {
                    costeG[vecino] = Mathf.Infinity;
                }

                // Hemos encontrado un camino mejor
                if (nuevoCosteG < costeG[vecino])
                {
                    padres[vecino] = actual;

                    costeG[vecino] = nuevoCosteG;

                    costeF[vecino] =
                        costeG[vecino] +
                        CalcularHeuristica(
                            vecino,
                            objetivo
                        );

                    if (!abiertos.Contains(vecino))
                    {
                        abiertos.Add(vecino);
                    }
                }
            }
        }

        // No existe ninguna ruta
        return new List<Waypoint>();
    }


    // Heurística H
    private float CalcularHeuristica(
        Waypoint actual,
        Waypoint objetivo)
    {
        return Vector3.Distance(
            actual.transform.position,
            objetivo.transform.position
        );
    }


    // Busca el nodo con menor F
    private Waypoint ObtenerMenorF(
        List<Waypoint> abiertos,
        Dictionary<Waypoint, float> costeF)
    {
        Waypoint mejor = abiertos[0];

        foreach (Waypoint waypoint in abiertos)
        {
            if (costeF[waypoint] < costeF[mejor])
            {
                mejor = waypoint;
            }
        }

        return mejor;
    }


    // Reconstruye el camino desde el objetivo hasta el inicio
    private List<Waypoint> ReconstruirRuta(
        Dictionary<Waypoint, Waypoint> padres,
        Waypoint objetivo)
    {
        List<Waypoint> ruta = new List<Waypoint>();

        Waypoint actual = objetivo;

        ruta.Add(actual);

        while (padres.ContainsKey(actual))
        {
            actual = padres[actual];
            ruta.Add(actual);
        }

        // La ruta se había construido al revés
        ruta.Reverse();

        return ruta;
    }
}