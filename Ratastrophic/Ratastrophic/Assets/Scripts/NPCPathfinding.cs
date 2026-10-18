
using System.Collections.Generic;
using UnityEngine;

public class NPCPathfinding : MonoBehaviour
{
    public Waypoint waypointInicial;
    public Waypoint waypointActual;
    public Waypoint objetivo;
    public Transform waypointDestino;
    private WaypointGraph grafo;

    
    public float velocidad = 3f;
    public float fuerzaMaxima = 5f;

    
    public float radioLlegada = 0.3f;
    public float distanciaPreparacion = 2f;
    public float velocidadGiro = 5f;

    public float radioFrenado = 5f;
    public bool esInvestigando = false;

    private Vector3 velocidadActual;

    private AStar aStar;
    private List<Waypoint> ruta;
    private int indiceRuta = 0;

    private bool puedeMoverse = false;
    private NPC_state state;
    public Transform player_tranform;

    private NPC_ObjectDetection objectDetection;

    private void Awake()//para que no de errores porque otros scripts necesitan las referencias
    {
        aStar = GetComponent<AStar>();
        state = GetComponent<NPC_state>();
        objectDetection = GetComponent<NPC_ObjectDetection>();

        if (aStar == null)
        {
            Debug.LogError("El NPC necesita el componente AStar.");
            return;
        }
    }

    private void Start()
    {
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
        Debug.Log("Vigilante - waypointInicial: " +
            (waypointInicial != null ? waypointInicial.name : "NULL"));//para ver por qu� dan error las referencias

        Debug.Log("Vigilante - waypointActual: " +
            (waypointActual != null ? waypointActual.name : "NULL"));
        Vector3 posicion = waypointActual.transform.position;//cambio para que no se hunda en el suelo
        posicion.y = transform.position.y;//cambio
        transform.position = posicion;//cambio

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

        if (aStar == null)
        {
            Debug.LogError("El vigilante no tiene una referencia v�lida a AStar.");
            return;
        }

        if (waypointActual == null)
        {
            Debug.LogError("El waypoint actual es null.");
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

        indiceRuta = ruta.Count > 1 ? 1 : 0; //cambio

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
        waypointDestino = transform;

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

            // SOLO amplia la ruta si NO esta investigando un ruido
            if (!esInvestigando && distanciaFinal <= distanciaPreparacion)
            {
                bool rutaPreparada = PrepararSiguienteRuta();

                if (rutaPreparada &&
                    Vector3.Distance(transform.position, waypointDestino.position) <= radioLlegada)
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
                ruta[indiceRuta].transform.position
            );

            if (distanciaWaypoint > radioLlegada)
                break;

            indiceRuta++;
        }

        if (indiceRuta >= ruta.Count)
            return;

        waypointDestino = ruta[indiceRuta].transform;



        Vector3 posicionObjetivo = waypointDestino.transform.position;
        Vector3 haciaObjetivo = posicionObjetivo - transform.position;
        Vector3 velocidadDeseada;

        bool esUltimoNode = (indiceRuta == ruta.Count - 1);

        if (esInvestigando && esUltimoNode)
        {
            // ARRIVAL
            velocidadDeseada = CalcularArrive(posicionObjetivo);
            print("usando arrival");
        }
        else
        {
            // SEEK
            velocidadDeseada = haciaObjetivo.sqrMagnitude > 0.001f
                ? haciaObjetivo.normalized * velocidad
                : Vector3.zero;
        }

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
    }

    public bool HaLlegadoADestino()//cambio
    {
        if (objetivo == null)
            return false;

        return Vector3.Distance(
            transform.position,
            objetivo.transform.position
        ) <= radioLlegada;
    }

    public void DetenerMovimiento()//cambio
    {
        puedeMoverse = false;
        velocidadActual = Vector3.zero;
    }

    private Vector3 CalcularArrive(Vector3 posicionObjetivo)
    {
        Vector3 haciaObjetivo = posicionObjetivo - transform.position;
        haciaObjetivo.y = 0f; 

        float distancia = haciaObjetivo.magnitude;
        print("distancia: " + distancia);
        Vector3 velocidadDeseada;

        if (distancia < radioFrenado)
        {
            float velocidadAtenuada = (distancia / radioFrenado) * velocidad;
            velocidadDeseada = haciaObjetivo.normalized * velocidadAtenuada;
        }
        else
        {
            velocidadDeseada = haciaObjetivo.normalized * velocidad;
        }
        print("Magnitud velocidad deseada: " + velocidadDeseada.magnitude);

        // Cuando la velocidad deseada sea muy baja o la distancia sea menor al radio de llegada
        if (distancia <= radioLlegada || velocidadDeseada.magnitude < 0.1f)
        {
            velocidadActual = Vector3.zero; //Se para
            state.NPC_currentState.NoiseReached();
            return Vector3.zero;
        }

        return velocidadDeseada;
    }

}
