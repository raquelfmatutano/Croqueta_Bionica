
using System.Collections.Generic;
using UnityEngine;

public class NPCPathfinding : MonoBehaviour
{
    public Waypoint waypointInicial;

    public Waypoint waypointActual;

    public Waypoint objetivo;

    private WaypointGraph grafo;

    
    public float velocidad = 3f;
    public float fuerzaMaxima = 5f;

    
    public float radioLlegada = 0.3f;
    public float distanciaPreparacion = 2f;
    public float velocidadGiro = 5f;

    private Vector3 velocidadActual;

    private AStar aStar;
    private List<Waypoint> ruta;
    private int indiceRuta = 0;

    private bool puedeMoverse = false;
    private bool deRuta = false;
    private NPC_state state;
    public Transform player_tranform;

    private NPC_ObjectDetection objectDetection;

    private void Start()
    {
        aStar = GetComponent<AStar>();
        state = GetComponent<NPC_state>();
        objectDetection = GetComponent<NPC_ObjectDetection>();

        if (aStar == null)
        {
            Debug.LogError("El NPC necesita el componente AStar.");
            return;
        }

        grafo = FindFirstObjectByType<WaypointGraph>();

        if (grafo == null)
        {
            Debug.LogError("No se ha encontrado ningun WaypointGraph.");
            return;
        }

        if (waypointInicial == null)
        {
            Debug.LogError("Debes asignar el waypoint inicial.");
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
            Debug.LogError("No se ha podido encontrar un nuevo waypoint.");
            return;
        }

        ruta = aStar.CalcularRuta(waypointActual, objetivo);

        if (ruta == null || ruta.Count == 0)
        {
            Debug.LogError("No se ha encontrado una ruta hasta " + objetivo.name);
            return;
        }

        indiceRuta = ruta.Count > 1 ? 1 : 0;

        Debug.Log("Nuevo destino: " + objetivo.name);
    }

    private bool PrepararSiguienteRuta()
    {
        if (state.NPC_currentState == RobotState.INVESTIGACION) {
            state.NPC_currentState = RobotState.PATRULLA;
        }
       
        Waypoint nuevoObjetivo =
            grafo.ObtenerWaypointAleatorio(objetivo);

        if (nuevoObjetivo == null)
            return false;

        List<Waypoint> nuevaRuta = aStar.CalcularRuta(
            objetivo,
            nuevoObjetivo
        );

        if (nuevaRuta == null || nuevaRuta.Count < 2)
            return false;

       
        for (int i = 1; i < nuevaRuta.Count; i++)
        {
            ruta.Add(nuevaRuta[i]);
        }

        objetivo = nuevoObjetivo;

        Debug.Log("Ruta ampliada. Nuevo destino: " + objetivo.name);

        return true;
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
        
            if (indiceRuta == ruta.Count - 1)
            {
                float distanciaFinal = Vector3.Distance(
                    transform.position,
                    waypointDestino.position
                );

                if (distanciaFinal <= distanciaPreparacion)
                {
                    bool rutaPreparada = PrepararSiguienteRuta();

                    if (rutaPreparada &&
                        Vector3.Distance(
                            transform.position,
                            waypointDestino.position
                        ) <= radioLlegada)
                    {
                        indiceRuta++;
                        
                    }
                }
            }

            if (indiceRuta >= ruta.Count)
                return;

            while (indiceRuta < ruta.Count - 1)
            {
                float distanciaWaypoint = Vector3.Distance(
                    transform.position,
                    waypointDestino.transform.position
                );

                if (distanciaWaypoint > radioLlegada)
                    break;

                indiceRuta++;
            }

            if (indiceRuta >= ruta.Count)
                return;
        }

        Vector3 posicionObjetivo = waypointDestino.transform.position;
        Vector3 haciaObjetivo = posicionObjetivo - transform.position;

        // SEEK
        Vector3 velocidadDeseada =
            haciaObjetivo.sqrMagnitude > 0.001f
                ? haciaObjetivo.normalized * velocidad
                : Vector3.zero;

        // STEERING 
        Vector3 fuerzaDireccion = velocidadDeseada - velocidadActual;

        if (objectDetection != null)
        {
            Vector3 avoidanceForce = objectDetection.CalculateAvoidanceForce();
            fuerzaDireccion += avoidanceForce * fuerzaMaxima;
        }

        fuerzaDireccion = Vector3.ClampMagnitude(
            fuerzaDireccion,
            fuerzaMaxima
        );

        velocidadActual += fuerzaDireccion * Time.deltaTime;

        velocidadActual = Vector3.ClampMagnitude(
            velocidadActual,
            velocidad
        );

        transform.position += velocidadActual * Time.deltaTime;

        // Girar hacia la direccion real del movimiento.
        Vector3 direccionMovimiento = velocidadActual;
        direccionMovimiento.y = 0f;

        if (direccionMovimiento.sqrMagnitude > 0.001f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(
                direccionMovimiento,
                Vector3.up
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadGiro * Time.deltaTime
            );
        }
    }

    public void ActivarMovimiento()
    {
        puedeMoverse = true;
        deRuta = true;
    }
}
