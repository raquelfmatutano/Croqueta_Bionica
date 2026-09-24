using System.Collections.Generic;
using UnityEngine;

public class NPCPathfinding : MonoBehaviour
{
    
    public Waypoint waypointInicial;

    public Waypoint waypointActual;

    public Waypoint objetivo;

    private WaypointGraph grafo;

    public float velocidad = 1f;

    private AStar aStar;

    private List<Waypoint> ruta;

    private int indiceRuta = 0;

    private bool puedeMoverse = false;
    private bool deRuta = false;
    private NPC_state state;
    public Transform player_tranform;

    private void Start()
    {
        aStar = GetComponent<AStar>();
        state = GetComponent<NPC_state>();

        if (aStar == null)
        {
            Debug.LogError(
                "El NPC necesita el componente AStar."
            );

            return;
        }

        grafo = FindFirstObjectByType<WaypointGraph>();

        if (grafo == null)
        {
            Debug.LogError(
                "No se ha encontrado ningun WaypointGraph en la escena."
            );

            return;
        }

        if (waypointInicial == null)
        {
            Debug.LogError(
                "Debes asignar el waypoint inicial del NPC."
            );

            return;
        }

        waypointActual = waypointInicial;

        transform.position = waypointActual.transform.position;

        ElegirNuevoDestino();
    }

    public void IrADestino(Waypoint nuevoObjetivo, Waypoint actual) {
        objetivo = nuevoObjetivo;
        waypointActual = actual;

        if (objetivo == null)
        {
            Debug.LogError(
                "No se ha podido encontrar un nuevo waypoint."
            );

            return;
        }

        ruta = aStar.CalcularRuta(
            waypointActual,
            objetivo
        );

        if (ruta == null || ruta.Count == 0)
        {
            Debug.LogError(
                "No se ha encontrado una ruta hasta " +
                objetivo.name
            );

            return;
        }

        indiceRuta = 0;

        Debug.Log(
            "Nuevo destino: " + objetivo.name
        );
    }

    private void ElegirNuevoDestino()
    {
        objetivo = grafo.ObtenerWaypointAleatorio(waypointActual);

        if (objetivo == null)
        {
            Debug.LogError(
                "No se ha podido encontrar un nuevo waypoint."
            );

            return;
        }

        ruta = aStar.CalcularRuta(
            waypointActual,
            objetivo
        );

        if (ruta == null || ruta.Count == 0)
        {
            Debug.LogError(
                "No se ha encontrado una ruta hasta " +
                objetivo.name
            );

            return;
        }

        indiceRuta = 0;

        Debug.Log(
            "Nuevo destino: " + objetivo.name
        );
    }


    private void Update()
    {
        Transform waypointDestino = transform;

        if (state.NPC_currentState == RobotState.PERSECUCION) {
            waypointDestino = player_tranform;
        }

        else {
            if (!puedeMoverse)
            return;

            if (ruta == null || ruta.Count == 0)
                return;

            if (indiceRuta >= ruta.Count)
                return;

            waypointDestino = ruta[indiceRuta].transform;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            waypointDestino.position,
            velocidad * Time.deltaTime
        );

        float distancia = Vector3.Distance(
            transform.position,
            waypointDestino.transform.position
        );

        if (distancia < 0.1f)
        {
            indiceRuta++;

            if (indiceRuta >= ruta.Count)
            {
                
                waypointActual = objetivo;

                Debug.Log(
                    "El NPC ha llegado a " +
                    waypointActual.name
                );

                deRuta = false;

                ElegirNuevoDestino();
                deRuta = true;
            }
        }
    }

    public void ActivarMovimiento()
    {
        puedeMoverse = true;
        deRuta = true;
    }
}