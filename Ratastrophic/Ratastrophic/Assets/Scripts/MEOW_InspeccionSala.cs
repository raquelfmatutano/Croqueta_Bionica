using UnityEngine;
using System.Collections.Generic;

public class MEOW_InspeccionSala : MonoBehaviour
{
    public NPC_vision visionNPC;
    public bool inspeccionEnCurso = false;
    public NPCPathfinding movimientoNPC;
    private WaypointGraph grafo;
    private List<Waypoint> waypointsSala = new List<Waypoint>();
    private List<Waypoint> waypointsVisitados = new List<Waypoint>();
    private List<int> idsSalas = new List<int>();
    private int indiceSalaActual = 0;
    private Dictionary<int, bool> resultadosPorSala = new Dictionary<int, bool>();//para guardar en qué sala ha visto al jugador

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

        if (!movimientoNPC.HaLlegadoADestino())
            return;

        if (!waypointsVisitados.Contains(movimientoNPC.objetivo))
        {
            waypointsVisitados.Add(movimientoNPC.objetivo);
        }

        IrAlSiguienteWaypoint();
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

            resultadosPorSala[idSalaTerminada] =
                visionNPC.jugadorVistoDuranteInspeccion;

            Debug.Log(
                "Sala " + idSalaTerminada +
                " terminada. Jugador visto: " +
                resultadosPorSala[idSalaTerminada]
            );

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

            visionNPC.IniciarInspeccion();

            IrAlSiguienteWaypoint();
            return;
        }

        Waypoint siguiente = pendientes[Random.Range(0, pendientes.Count)];

        Waypoint actual = movimientoNPC.objetivo != null
            ? movimientoNPC.objetivo
            : movimientoNPC.waypointActual;

        movimientoNPC.IrADestino(siguiente, actual);
    }
}
