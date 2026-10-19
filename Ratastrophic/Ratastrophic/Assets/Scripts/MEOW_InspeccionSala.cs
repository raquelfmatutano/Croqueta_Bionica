using UnityEngine;
using System.Collections.Generic;

public class MEOW_InspeccionSala : MonoBehaviour
{
    public NPC_vision visionNPC;
    public bool inspeccionEnCurso = false;
    public MEOW_Pathfinding movimientoNPC;
    public List<MEOW_CartelEntrada> todosLosCarteles = new List<MEOW_CartelEntrada>();
    private WaypointGraph grafo;
    private List<Waypoint> waypointsSala = new List<Waypoint>();
    private List<Waypoint> waypointsVisitados = new List<Waypoint>();
    private List<int> idsSalas = new List<int>();
    private int indiceSalaActual = 0;
    private Dictionary<int, bool> resultadosPorSala = new Dictionary<int, bool>();//para guardar en que sala ha visto al jugador
    private bool desplazandoseEntreSalas = false;
    

    private void Start()
    {
        grafo = FindFirstObjectByType<WaypointGraph>();

        if (grafo == null)
        {
            Debug.LogError("No se ha encontrado WaypointGraph.");
            return;
        }

        idsSalas = grafo.ObtenerIdsSalas();
        idsSalas.Sort();

        if (idsSalas.Count == 0)
        {
            Debug.LogError("No se han encontrado habitaciones.");
        }
        else
        {
            Debug.Log("Habitaciones encontradas: " + idsSalas.Count);
        }

        
    }

    
    private void CargarSalaActual()
    {
        if (indiceSalaActual >= idsSalas.Count)
        {
            TerminarInspeccion();
            return;
        }

        int id = idsSalas[indiceSalaActual];

        waypointsSala = grafo.ObtenerWaypointsSala(id);
        waypointsVisitados.Clear();

        Debug.Log("Inspeccionando sala con ID: " + id);
    }



    private void Update()
    {
        if (!inspeccionEnCurso &&
            movimientoNPC != null &&
            movimientoNPC.waypointActual != null)
        {
            ComenzarInspeccion();
        }

        if (!inspeccionEnCurso)
            return;

    }

    public void ComenzarInspeccion()
    {
        if (movimientoNPC == null || visionNPC == null)
        {
            Debug.LogError("Faltan referencias del vigilante.");
            return;
        }

        if (idsSalas.Count == 0)
        {
            Debug.LogError("No hay habitaciones para inspeccionar.");
            return;
        }

        indiceSalaActual = 0;
        CargarSalaActual();

        if (waypointsSala.Count == 0)
        {
            Debug.LogError("La primera sala no tiene waypoints.");
            return;
        }

        inspeccionEnCurso = true;
        waypointsVisitados.Clear();

        visionNPC.ActivarVision();
        visionNPC.IniciarInspeccion();

        if (waypointsSala.Contains(movimientoNPC.waypointActual))
        {
            waypointsVisitados.Add(movimientoNPC.waypointActual);
        }

        movimientoNPC.ActivarMovimiento();

        IrAlSiguienteWaypoint();

        Debug.Log("Inspección iniciada");
    }


    public void TerminarInspeccion()
    {
        inspeccionEnCurso = false;
        movimientoNPC.DetenerMovimiento();

        Debug.Log("===== RESUMEN DE INSPECCIÓN =====");

        foreach (KeyValuePair<int, bool> resultado in resultadosPorSala)
        {
            Debug.Log(
                "Sala " + resultado.Key +
                " | Jugador visto: " + resultado.Value
            );
        }

        Debug.Log("Inspección de todas las salas terminada.");
    }



    private void IrAlSiguienteWaypoint()
    {
        List<Waypoint> pendientes = new List<Waypoint>();

        foreach (Waypoint waypoint in waypointsSala)
        {
            if (!waypointsVisitados.Contains(waypoint))
            {
                pendientes.Add(waypoint);
            }
        }

        if (pendientes.Count == 0)
        {
            int idSalaTerminada = idsSalas[indiceSalaActual];
            visionNPC.DesactivarVision();

            resultadosPorSala[idSalaTerminada] =
                visionNPC.jugadorVistoDuranteInspeccion;

            Debug.Log(
                "Sala " + idSalaTerminada +
                " terminada. Jugador visto: " +
                resultadosPorSala[idSalaTerminada]
            );

            //Colocar carteles
            if (!resultadosPorSala[idSalaTerminada])
            {
                foreach (MEOW_CartelEntrada cartel in todosLosCarteles)
                {
                    if (cartel.PerteneceASala(idSalaTerminada) && !cartel.YaColocado)
                    {
                        cartel.ColocarCartel();
                    }
                }
            }

            indiceSalaActual++;

            if (indiceSalaActual >= idsSalas.Count)
            {
                TerminarInspeccion();
                return;
            }

            CargarSalaActual();

            if (waypointsSala.Count == 0)
            {
                IrAlSiguienteWaypoint();
                return;
            }

            desplazandoseEntreSalas = true;
            visionNPC.DesactivarVision();

            IrAlSiguienteWaypoint();
            return;
        }

        Waypoint siguiente = pendientes[Random.Range(0, pendientes.Count)];

        Waypoint actual = movimientoNPC.waypointActual;

        movimientoNPC.IrADestino(siguiente, actual);
    }

    private void OnEnable()
    {
        if (movimientoNPC != null)
            movimientoNPC.AlcanzoWaypoint += GestionarWaypointAlcanzado;
    }

    private void OnDisable()
    {
        if (movimientoNPC != null)
            movimientoNPC.AlcanzoWaypoint -= GestionarWaypointAlcanzado;
    }

    private void ComprobarEntradaSala(Waypoint waypoint)
    {
        if (!desplazandoseEntreSalas)
            return;

        if (indiceSalaActual < 0 || indiceSalaActual >= idsSalas.Count)
            return;

        int idSalaObjetivo = idsSalas[indiceSalaActual];

        if (waypoint == null || waypoint.idSala != idSalaObjetivo)
            return;

        desplazandoseEntreSalas = false;

        visionNPC.ActivarVision();
        visionNPC.IniciarInspeccion();

        waypointsVisitados.Clear();
        waypointsVisitados.Add(waypoint);

        Debug.Log("Gato ha entrado en la sala " + idSalaObjetivo +
                  " por el waypoint " + waypoint.name);
    }

    private void GestionarWaypointAlcanzado(Waypoint waypoint)
    {
       if (!inspeccionEnCurso || waypoint == null)
            return;

        ComprobarEntradaSala(waypoint);

        if (waypoint != movimientoNPC.objetivo)
            return;

        if (!waypointsVisitados.Contains(waypoint))
        {
            waypointsVisitados.Add(waypoint);
        }

        IrAlSiguienteWaypoint();
    }
}
