using UnityEngine;
using System.Collections.Generic;

public class MEOW_InspeccionSala : MonoBehaviour
{
    public NPC_vision visionNPC;
    public bool inspeccionEnCurso = false;
    public NPCPathfinding movimientoNPC;
    public int idSala;
    private WaypointGraph grafo;
    private List<Waypoint> waypointsSala = new List<Waypoint>();
    private List<Waypoint> waypointsVisitados = new List<Waypoint>();

    private void Start()
    {
        grafo = FindFirstObjectByType<WaypointGraph>();

        if (grafo == null)
        {
            Debug.LogError("No se ha encontrado WaypointGraph.");
            return;
        }

        waypointsSala = grafo.ObtenerWaypointsSala(idSala);

        if (waypointsSala.Count == 0)
        {
            Debug.LogError("No hay waypoints asignados a esta sala.");
        }
    }

    private void Update()
    {
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

        if (waypointsSala.Count == 0)
        {
            Debug.LogError("La sala no tiene nodos para inspeccionar.");
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

        if (visionNPC.PuedeColocarCartel())
        {
            Debug.Log("El jugador no ha sido visto. Se puede colocar el cartel.");
        }
        else
        {
            Debug.Log("El jugador ha sido visto. No se coloca el cartel.");
        }
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
            TerminarInspeccion();
            return;
        }

        Waypoint siguiente = pendientes[Random.Range(0, pendientes.Count)];

        Waypoint actual = movimientoNPC.objetivo != null
            ? movimientoNPC.objetivo
            : movimientoNPC.waypointActual;

        movimientoNPC.IrADestino(siguiente, actual);
    }
}
