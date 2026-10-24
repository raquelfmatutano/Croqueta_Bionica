
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
    public NPC_state state;
    public Transform player_tranform;

    public int waypoints_until_charge;
    public bool going_to_charge = false;
    //public bool reached_station = false;

    private NPC_ObjectDetection objectDetection;

    private void Awake()//para que no de errores porque otros scripts necesitan las referencias
    {
        aStar = GetComponent<AStar>();
        if(state == null)
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
            (waypointInicial != null ? waypointInicial.name : "NULL"));

        Debug.Log("Vigilante - waypointActual: " +
            (waypointActual != null ? waypointActual.name : "NULL"));
        Vector3 posicion = waypointActual.transform.position;
        posicion.y = transform.position.y;
        transform.position = posicion;

        waypoints_until_charge = Random.Range(1, 3);

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
            Debug.LogError("El vigilante no tiene una referencia valida a AStar.");
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

        indiceRuta = ruta.Count > 1 ? 1 : 0;

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

        if (state.NPC_currentState is PerseguirState) {
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
        
        if (indiceRuta == ruta.Count - 1) //final de trayecto
        {
            //mandar a cargar
            if (waypoints_until_charge <= 0) 
            {
                if (!going_to_charge)
                {
                    if (GameManager.Instance != null && GameManager.Instance.charging_station_available)
                    {
                        state.NPC_currentState.NeedsCharging(true);
                    }
                    else 
                    {
                        print("charging station occupied. i can go for a bit longer");
                        waypoints_until_charge = Random.Range(5, 10);
                    }
                }
                else 
                {
                    if (!GameManager.Instance.timer_on && !GameManager.Instance.timer_finished) 
                    {
                        state.NPC_currentState.StartCharging();
                        return;
                    }
                }
            }

            state.NPC_currentState.NoiseReached(); //en caso de haber llegado a un sonido
        
            float distanciaFinal = Vector3.Distance(
                transform.position,
                waypointDestino.position
            );

            if (distanciaFinal <= distanciaPreparacion)
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

            indiceRuta++; //ha pasado por un waypoint
            waypoints_until_charge--;
        }

        if (indiceRuta >= ruta.Count)
            return;

        waypointDestino = ruta[indiceRuta].transform;
        }

        Vector3 posicionObjetivo = waypointDestino.transform.position;
        Vector3 haciaObjetivo = posicionObjetivo - transform.position;
        Vector3 velocidadDeseada;

        bool esUltimoNode;
        if(ruta.Count >= 2)
            esUltimoNode = (indiceRuta == ruta.Count - 2);
        else
            esUltimoNode = (indiceRuta == ruta.Count - 1);

        
        if (going_to_charge && esUltimoNode)
        {
            // ARRIVAL
            velocidadDeseada = CalcularArrive(posicionObjetivo);
        }
        else    
        {
            // SEEK
            velocidadDeseada = haciaObjetivo.sqrMagnitude > 0.001f
                ? haciaObjetivo.normalized * velocidad
                : Vector3.zero;
        }

        if (velocidadDeseada == Vector3.zero)
        {
            velocidadActual = Vector3.zero;
            return; 
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
        print("movimiento activado");
        puedeMoverse = true;
    }

    public bool HaLlegadoADestino()
    {
        if (objetivo == null)
            return false;

        return Vector3.Distance(
            transform.position,
            objetivo.transform.position
        ) <= radioLlegada;
    }

    public void DetenerMovimiento()
    {
        print("movimiento desactivado");
        puedeMoverse = false;
        velocidadActual = Vector3.zero;
    }

    private Vector3 CalcularArrive(Vector3 posicionObjetivo)
    {
        Vector3 haciaObjetivo = posicionObjetivo - transform.position;
        haciaObjetivo.y = 0f; 

        float distancia = haciaObjetivo.magnitude;
        //print("distancia: " + distancia);
        Vector3 velocidadDeseada;

        float velocidadAtenuada = (distancia / radioFrenado) * velocidad;
        velocidadDeseada = haciaObjetivo.normalized * velocidadAtenuada;

        //print("Magnitud velocidad deseada: " + velocidadDeseada.magnitude);

        float velocidadCorte = velocidad * 0.15f;
        // Cuando la velocidad deseada sea muy baja o la distancia sea menor al radio de llegada
        if (distancia <= radioLlegada || velocidadDeseada.magnitude < velocidadCorte)
        {
            velocidadActual = Vector3.zero; //Se para
            if (!GameManager.Instance.timer_on && !GameManager.Instance.timer_finished) 
            {
                state.NPC_currentState.StartCharging();
                return Vector3.zero;
            }

            return Vector3.zero;
        }

        return velocidadDeseada;
    }

}
