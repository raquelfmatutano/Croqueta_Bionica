using System.Collections.Generic;
using UnityEngine;

public class NPCPathfinding : MonoBehaviour
{
    [Header("Waypoints")]
    private Waypoint inicio;
    private Waypoint objetivo;

    [Header("Movimiento")]
    public float velocidad = 3f;

    private AStar aStar;

    private List<Waypoint> ruta;

    private int indiceRuta = 0;

    private void Start()
    {

        inicio = GameObject.Find("F").GetComponent<Waypoint>();
        objetivo = GameObject.Find("D").GetComponent<Waypoint>();
        
        aStar = GetComponent<AStar>();

        if (aStar == null)
        {
            Debug.LogError(
                "El NPC necesita el componente AStar."
            );

            return;
        }

        // Comprobamos que tenemos los waypoints
        if (inicio == null || objetivo == null)
        {
            Debug.LogError(
                "Debes asignar el waypoint inicial y el objetivo."
            );

            return;
        }

        // Calculamos la ruta utilizando A*
        ruta = aStar.CalcularRuta(
            inicio,
            objetivo
        );

        if (ruta.Count == 0)
        {
            Debug.LogError(
                "No se ha encontrado una ruta."
            );

            return;
        }

        Debug.Log(
            "Ruta encontrada con " +
            ruta.Count +
            " waypoints."
        );
    }


    private void Update()
    {
        if (ruta == null || ruta.Count == 0)
            return;

        if (indiceRuta >= ruta.Count)
            return;

        // Waypoint al que nos dirigimos
        Waypoint waypointActual =
            ruta[indiceRuta];

        // Movemos el NPC hacia el waypoint
        transform.position = Vector3.MoveTowards(
            transform.position,
            waypointActual.transform.position,
            velocidad * Time.deltaTime
        );

        // Comprobamos si hemos llegado
        float distancia = Vector3.Distance(
            transform.position,
            waypointActual.transform.position
        );

        if (distancia < 0.1f)
        {
            indiceRuta++;

            // Hemos terminado la ruta
            if (indiceRuta >= ruta.Count)
            {
                Debug.Log(
                    "El NPC ha llegado al objetivo."
                );
            }
        }
    }
}