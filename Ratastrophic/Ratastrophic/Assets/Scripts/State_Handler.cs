using UnityEngine;


public class PatrullarState : State
{
    public PatrullarState(NPC_state npc) : base(npc) { }

    public override void Enter()
    {
        Debug.Log("Entering Patrol State");
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
}


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
        }
    }

    public override void Update()
    {
        if (npc.seesPlayer)
        {
            npc.ChangeState(npc.perseguirState);
            return;
        }

    }

    public override void Exit() { }
}

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

public class LimpiarState : State
{
    public LimpiarState(NPC_state npc) : base(npc) { }

    public override void Enter()
    {
        Debug.Log("Entering cleaning state");
    }

    public override void Update()
    {
        npc.pathfinder.waypointDestino = npc.currentInterest;
        Debug.Log("Gotta clean gotta clean");

    }

    public override void Exit() { 
        
    } 

    public override void NoiseReached() {
        npc.ChangeState(npc.patrullarState);
    }
}