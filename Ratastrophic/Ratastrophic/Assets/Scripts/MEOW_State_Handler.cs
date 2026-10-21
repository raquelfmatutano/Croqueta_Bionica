using UnityEngine;

public class InspeccionarState : State
{
    private MEOW_NPC_state meow;
    

    public InspeccionarState(MEOW_NPC_state npc) : base(npc)
    {
        this.meow = npc;
    }

    public override void Enter()
    {
        Debug.Log("Gato FSM: Entrando a INSPECCIONAR");

        if (meow.visionNPC != null)
            meow.visionNPC.ActivarVision();

        if (meow.inspeccionSala != null)
        {
            meow.inspeccionSala.IniciarOContinuarInspeccionSala();
        }
    }

    public override void Update()
    {
        if (meow.seesPlayer)
        {
            PlayerSpotted();
            return;
        }

        if (meow.inspeccionSala != null && meow.inspeccionSala.SalaActualTerminada())
        {
            if (meow.inspeccionSala.QuedanSalasPendientes())
            {
                meow.ChangeState(meow.trasladarState);
            }
            else
            {
                meow.inspeccionSala.TerminarInspeccion();
            }
        }
    }

    public override void PlayerSpotted()
    {
        Debug.Log("Gato FSM: ¡Jugador visto! Cambiando a ALERTAR");
        meow.waypointJugadorVisto = meow.grafo.find_nearest_waypoint(meow.transform.position);//posicion del gato cuando ve al jugador
        meow.ChangeState(meow.alertarState);
    }

    public override void Exit() { }
}


public class TrasladarState : State
{
    private MEOW_NPC_state meow;

    public TrasladarState(MEOW_NPC_state npc) : base(npc)
    {
        this.meow = npc;
    }

    public override void Enter()
    {
        Debug.Log("Gato FSM: Entrando a TRASLADAR");

        if (meow.visionNPC != null)
            meow.visionNPC.DesactivarVision();

        if (meow.inspeccionSala != null)
        {
            meow.inspeccionSala.ViajarASiguienteSala();
        }
    }

    public override void Update()
    {
        
        if (meow.inspeccionSala != null && meow.inspeccionSala.EntroEnNuevaSala)
        {
            meow.ChangeState(meow.inspeccionarState);
        }
    }

    public override void Exit() { }
}

public class AlertarState : State
{
    private MEOW_NPC_state meow;
    private Transform robotObjetivo;
    private bool robotAvisado;
    private float tiempoSiguienteRecalculo = 0f;
    private float intervaloRecalculo = 0.5f; // Comprueba y recalcula cada 0.5 segundos

    public AlertarState(MEOW_NPC_state npc) : base(npc)
    {
        this.meow = npc;
    }

    public override void Enter()
    {
        Debug.Log("Gato FSM: Entrando a ALERTAR (Buscando robot en PatrullarState)");
        robotAvisado = false;
        tiempoSiguienteRecalculo = 0f;

        if (meow.visionNPC != null)
            meow.visionNPC.DesactivarVision();

        robotObjetivo = BuscarRobotMasCercanoLibre();

        if (robotObjetivo == null)
        {
            Debug.Log("Gato FSM: No hay robots patrullando. Pasando a TRASLADAR...");
            robotAvisado = true;
            meow.ChangeState(meow.trasladarState);
        }
    }

    public override void Update()
    {
        if (robotAvisado || robotObjetivo == null) return;

        if (Time.time >= tiempoSiguienteRecalculo)
        {
            tiempoSiguienteRecalculo = Time.time + intervaloRecalculo;

            NPC_state robotState = robotObjetivo.GetComponent<NPC_state>();

            if (robotState == null || !(robotState.NPC_currentState is PatrullarState))
            {
                Debug.Log("Gato FSM: El robot ya no está en PatrullarState. Buscando otro...");
                robotObjetivo = BuscarRobotMasCercanoLibre();

                if (robotObjetivo == null)
                {
                    Debug.Log("Gato FSM: No hay ningún robot disponible en patrulla. Cambiando a TRASLADAR.");
                    robotAvisado = true;
                    meow.ChangeState(meow.trasladarState);
                    return;
                }
            }

            // Actualizamos la ruta hacia la posicion del robot
            Waypoint waypointRobot = meow.grafo.find_nearest_waypoint(robotObjetivo.position);
            Waypoint inicio = meow.grafo.find_nearest_waypoint(meow.transform.position);

            if (waypointRobot != null && inicio != null)
            {
                meow.pathfinder.IrADestino(waypointRobot, inicio);
            }
        }

        float distancia = Vector3.Distance(meow.transform.position, robotObjetivo.position);
        if (distancia <= 2.5f)
        {
            AvisarAlRobot();
        }
    }

    private void AvisarAlRobot()
    {
        robotAvisado = true;
        Debug.Log("Gato FSM: ¡Robot en patrulla alcanzado! Transmitiendo alerta por sonido...");

        Waypoint waypointAlerta = meow.waypointJugadorVisto;

        Collider[] hitColliders = Physics.OverlapSphere(meow.transform.position, 4f);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("NPC") && hit.gameObject != meow.gameObject)
            {
                NPC_hearing otherNPC = hit.GetComponent<NPC_hearing>();
                if (otherNPC != null)
                {
                    otherNPC.ReceiveAlert(waypointAlerta);
                }
            }
        }

        meow.ChangeState(meow.trasladarState);
    }

    private Transform BuscarRobotMasCercanoLibre()
    {
        GameObject[] robots = GameObject.FindGameObjectsWithTag("NPC");
        Transform masCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (GameObject robot in robots)
        {
            if (robot != meow.gameObject)
            {
                NPC_state robotState = robot.GetComponent<NPC_state>();

                if (robotState == null || !(robotState.NPC_currentState is PatrullarState))
                {
                    continue;
                }

                float dist = Vector3.Distance(meow.transform.position, robot.transform.position);
                if (dist < distanciaMinima)
                {
                    distanciaMinima = dist;
                    masCercano = robot.transform;
                }
            }
        }

        return masCercano;
    }

    public override void Exit() { }
}