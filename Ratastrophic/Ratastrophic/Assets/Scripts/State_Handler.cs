using UnityEngine;


public class PatrullarState : State
{
    public PatrullarState(NPC_state npc) : base(npc) { }

    public override void Enter()
    {
        npc.pathfinder.esInvestigando = false;
    }

    public override void Update()
    {

    }

    public override void NoiseHeard(Waypoint location)
    {
        npc.investigarState.SetTarget(location);
        npc.ChangeState(npc.investigarState);
    }

    public override void PlayerSpotted()
    {
        npc.ChangeState(npc.perseguirState);
    }

    public override void Exit() { }

    public override void InterestSpotted(Transform obj) {
        npc.currentInterest = obj;
        npc.ChangeState(npc.limpiarState);
    }

    public override void NeedsCharging(bool needs) {
        if (needs) {
            npc.ChangeState(npc.cargarState);
            GameManager.Instance.charging_station_available = false;
            npc.pathfinder.going_to_charge = true;
        }
            
    }
}

//-----------------------------------------------------------------
//-----------------------------------------------------------------


public class InvestigarState : State
{
    public InvestigarState(NPC_state npc) : base(npc) { }
    private Waypoint targetWaypoint;

    public void SetTarget(Waypoint target)
    {
        targetWaypoint = target;
    }

    public override void InterestSpotted(Transform obj) {
        npc.currentInterest = obj;
        npc.ChangeState(npc.limpiarState);
    }

    public override void NoiseHeard(Waypoint location)
    {
        SetTarget(location);
        Enter();
    }

    public override void NoiseReached() {
        npc.pathfinder.esInvestigando = false;
        npc.ChangeState(npc.patrullarState);
    }

    public override void PlayerSpotted()
    {
        npc.ChangeState(npc.perseguirState);
    }

    public override void Enter()
    {
        if (targetWaypoint != null)
        {
            Waypoint start = npc.grafo.find_nearest_waypoint(npc.transform.position);
            npc.pathfinder.IrADestino(targetWaypoint, start);
            npc.pathfinder.esInvestigando = true;
        }
    }

    public override void Update()
    {
        npc.pathfinder.waypointDestino = targetWaypoint.transform;
    }

    public override void Exit() { }
}

//-----------------------------------------------------------------
//-----------------------------------------------------------------

public class PerseguirState : State
{
    public PerseguirState(NPC_state npc) : base(npc) { }

    public override void Enter()
    {
        Debug.Log("Entering Chase State");
    }

    public override void Update()
    {
        npc.pathfinder.waypointDestino = npc.pathfinder.player_tranform;
    }

    public override void Exit() { }

    public override void PlayerLost()
    {
        npc.ChangeState(npc.patrullarState);
    }
}

//-----------------------------------------------------------------
//-----------------------------------------------------------------

public class LimpiarState : State
{
    public LimpiarState(NPC_state npc) : base(npc) { }

    public override void Enter()
    {
        npc.pathfinder.esInvestigando = false;
        Debug.Log("Entering cleaning state");
        npc.activeVision = false;
    }

    public override void Update()
    {
        npc.pathfinder.waypointDestino = npc.currentInterest;
        Debug.Log("Gotta clean gotta clean");

    }

    public override void Exit() { 
        npc.activeVision = true;
    } 

    public override void NoiseReached() {
        npc.ChangeState(npc.patrullarState);
    }
}

//-----------------------------------------------------------------
//-----------------------------------------------------------------

public class CargarState : State 
{
    public CargarState(NPC_state npc) : base(npc) { }

    private bool isCharging;

    public override void Enter()
    {
        isCharging = false;

        Waypoint start = npc.grafo.find_nearest_waypoint(npc.transform.position);
        npc.pathfinder.IrADestino(npc.grafo.charging_station, start);
        npc.activeVision = false;
    }

    public override void Update()
    {
       if(isCharging) {
            //Currently charging
            if (GameManager.Instance.timer_on) 
            {
                return;
            }

            //Finished charging
            else if (GameManager.Instance.timer_finished) 
            {
                Debug.Log("Charged");
                npc.pathfinder.ActivarMovimiento();
                npc.pathfinder.waypoints_until_charge = Random.Range(10, 15);
                npc.pathfinder.going_to_charge = false;
                
                GameManager.Instance.charging_station_available = true;
                GameManager.Instance.timer_finished = false;

                npc.ChangeState(npc.patrullarState);
            }
       }
    }

    public override void Exit() { 
        npc.activeVision = true;
        Waypoint start = npc.grafo.find_nearest_waypoint(npc.transform.position);
        Waypoint end = npc.grafo.ObtenerWaypointAleatorio(npc.pathfinder.waypointActual);
        npc.pathfinder.IrADestino(end, start);
    } 

    public override void NeedsCharging(bool needs) {
        if(!needs) {
            npc.ChangeState(npc.patrullarState);
        }
    }

    public override void StartCharging() {
        GameManager.Instance.ChargingTimer();
        Debug.Log("Starting charge");
        npc.pathfinder.DetenerMovimiento();
        isCharging = true;
    }
}