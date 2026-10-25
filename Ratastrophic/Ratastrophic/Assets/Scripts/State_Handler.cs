using UnityEngine;


public class PatrullarState : State
{
    public PatrullarState(NPC_state npc) : base(npc) { }

    public override void Enter()
    {
        npc.pathfinder.esInvestigando = false;
        Debug.Log("Entering Patrol State");
        npc.ChangeAnimation(0);
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

    /*public override void InterestSpotted(Transform obj) {
        npc.currentInterest = obj;
        npc.ChangeState(npc.limpiarState);
    }*/

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
        npc.ChangeAnimation(0);
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
    private float end_timer;
    private bool lost_timer_on;

    public override void Enter()
    {
        lost_timer_on = false;
        Debug.Log("Entering Chase State");
        npc.ChangeAnimation(0);
    }

    public override void Update()
    {
        npc.pathfinder.waypointDestino = npc.pathfinder.player_tranform;

        if (lost_timer_on) {
            if(Time.time > end_timer) 
                npc.ChangeState(npc.patrullarState);
            
            else
                Debug.Log(end_timer - Time.time);
        }


    }

    public override void Exit() { }

    public override void PlayerSpotted() {
        lost_timer_on = false;
    }

    public override void PlayerLost()
    {
        lost_timer_on = true;
        end_timer = Time.time + 3f;
    }
}

//-----------------------------------------------------------------
//-----------------------------------------------------------------

public class LimpiarState : State
{
    public LimpiarState(NPC_state npc) : base(npc) { }
    private bool isCleaning;

    private float end_timer;

    public override void Enter()
    {
        if(!npc.currentInterest.GetComponent<NoiseObject>().clean)
            npc.currentInterest.GetComponent<NoiseObject>().clean = true;
        isCleaning = false;
        npc.pathfinder.esInvestigando = false;
        Debug.Log("Entering cleaning state");
        npc.activeVision = false;
        npc.ChangeAnimation(1);
    }

    public override void Update()
    {
        npc.pathfinder.waypointDestino = npc.currentInterest;

        if(isCleaning) {
            //Currently cleaning
            if (Time.time < end_timer) 
            {
                Debug.Log(end_timer - Time.time);
                return;
            }

            //Finished cleaning
            else
            {
                Debug.Log("Finished cleaning");

                if (npc.currentInterest.GetComponent<NoiseObject>() != null) {
                    npc.currentInterest.GetComponent<NoiseObject>().CleanObject();
                }
                
                npc.pathfinder.ActivarMovimiento();
                npc.ChangeState(npc.patrullarState);
            }
       }

    }

    public override void StartCleaning() {
        isCleaning = true;
        GameManager.Instance.ChargingTimer();
        Debug.Log("Starting cleaning");
        npc.pathfinder.DetenerMovimiento();

        end_timer = Time.time + Random.Range(2f, 4f);
        Debug.Log(end_timer);
    }

    public override void Exit() { 
        npc.activeVision = true;
        npc.activeVision = true;

        npc.currentInterest = null;

        Waypoint start = npc.grafo.find_nearest_waypoint(npc.transform.position);
        Waypoint end = npc.grafo.ObtenerWaypointAleatorio(npc.pathfinder.waypointActual);
        npc.pathfinder.IrADestino(end, start);
        npc.ChangeAnimation(0);
    } 

    /*public override void NoiseReached() {
        npc.ChangeState(npc.patrullarState);
    }*/
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
        npc.ChangeAnimation(1);
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