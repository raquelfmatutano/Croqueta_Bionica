using UnityEngine;
using System.Collections.Generic;

public class MEOW_InspeccionSala : MonoBehaviour
{
    public NPC_vision visionNPC;
    public bool inspeccionEnCurso = false;
    public MEOW_Pathfinding movimientoNPC;
    public List<MEOW_CartelEntrada> todosLosCarteles = new List<MEOW_CartelEntrada>();

    public bool EntroEnNuevaSala { get; private set; } = false;
    private bool desplazandoseEntreSalas = false;

    private WaypointGraph grafo;
    private List<Waypoint> waypointsSala = new List<Waypoint>();
    private List<Waypoint> waypointsVisitados = new List<Waypoint>();
    private List<int> idsSalas = new List<int>();
    private int indiceSalaActual = 0;
    private Dictionary<int, bool> resultadosPorSala = new Dictionary<int, bool>();

 

    private void Awake()
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

        if (waypointsSala.Contains(movimientoNPC.waypointActual))
        {
            waypointsVisitados.Add(movimientoNPC.waypointActual);
        }

        movimientoNPC.ActivarMovimiento();
        IrAlSiguienteWaypoint();

        Debug.Log("Inspección iniciada");
    }

    public void IniciarOContinuarInspeccionSala()
    {
        inspeccionEnCurso = true;
        if (movimientoNPC != null) movimientoNPC.ActivarMovimiento();

        if (waypointsSala.Count == 0 && idsSalas.Count > 0)
        {
            indiceSalaActual = 0;
            CargarSalaActual();
        }

        if (movimientoNPC != null && (movimientoNPC.objetivo == null || movimientoNPC.HaLlegadoADestino()))
        {
            IrAlSiguienteWaypoint();
        }
    }

    public bool SalaActualTerminada()
    {
        if (waypointsSala.Count == 0) return true;

        foreach (Waypoint wp in waypointsSala)
        {
            if (!waypointsVisitados.Contains(wp)) return false;
        }
        return true;
    }

    public bool QuedanSalasPendientes()
    {
        return indiceSalaActual < idsSalas.Count - 1;
    }

    // Método llamado por la FSM al entrar en TrasladarState
    public void ViajarASiguienteSala()
    {
        if (indiceSalaActual >= idsSalas.Count) return;

        int idSalaTerminada = idsSalas[indiceSalaActual];

        // Guardar resultado y colocar carteles si la sala quedó limpia
        bool visto = visionNPC != null && visionNPC.jugadorVistoDuranteInspeccion;
        resultadosPorSala[idSalaTerminada] = visto;

        if (!visto)
        {
            foreach (MEOW_CartelEntrada cartel in todosLosCarteles)
            {
                if (cartel != null && cartel.PerteneceASala(idSalaTerminada) && !cartel.YaColocado)
                {
                    cartel.ColocarCartel();
                }
            }
        }

        // Pasar a la siguiente sala
        indiceSalaActual++;

        if (!QuedanSalasPendientes() && indiceSalaActual >= idsSalas.Count)
        {
            TerminarInspeccion();
            return;
        }

        CargarSalaActual();

        if (waypointsSala.Count > 0)
        {
            desplazandoseEntreSalas = true;
            EntroEnNuevaSala = false;
            movimientoNPC.IrADestino(waypointsSala[0], movimientoNPC.waypointActual);
        }
    }

    public void TerminarInspeccion()
    {
        inspeccionEnCurso = false;
        if (movimientoNPC != null) movimientoNPC.DetenerMovimiento();

        Debug.Log("===== RESUMEN DE INSPECCIÓN =====");

        foreach (KeyValuePair<int, bool> resultado in resultadosPorSala)
        {
            Debug.Log("Sala " + resultado.Key + " | Jugador visto: " + resultado.Value);
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

        // Si ya no quedan pendientes en esta sala, detenemos el avance automático y dejamos que la FSM gestione el cambio de estado
        if (pendientes.Count == 0)
        {
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

    private void GestionarWaypointAlcanzado(Waypoint waypoint)
    {
        if (!inspeccionEnCurso || waypoint == null)
            return;

        if (desplazandoseEntreSalas && indiceSalaActual < idsSalas.Count)
        {
            int idSalaObjetivo = idsSalas[indiceSalaActual];

            if (waypoint.idSala == idSalaObjetivo)
            {
                desplazandoseEntreSalas = false;
                EntroEnNuevaSala = true;

                waypointsVisitados.Clear();
                waypointsVisitados.Add(waypoint);

                Debug.Log("Gato entra en la sala " + idSalaObjetivo + " en waypoint " + waypoint.name);

                // Búsqueda inmediata del siguiente waypoint de la sala
                IrAlSiguienteWaypoint();
                return;
            }
        }

        if (waypoint != movimientoNPC.objetivo)
            return;

        if (!waypointsVisitados.Contains(waypoint))
        {
            waypointsVisitados.Add(waypoint);
        }

        IrAlSiguienteWaypoint();
    }
}