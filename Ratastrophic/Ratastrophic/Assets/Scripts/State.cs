using UnityEngine;

public abstract class State
{
    protected NPC_state npc;

    public State(NPC_state npc)
    {
        this.npc = npc;
    }

    public abstract void Enter(); 
    public abstract void Update();
    public abstract void Exit();

    public virtual void NoiseHeard(Waypoint location) { }
    public virtual void InterestSpotted(Transform obj) { }
    public virtual void NoiseReached() { }
    public virtual void PlayerSpotted() { }
    public virtual void PlayerLost() { }

    public virtual void NeedsCharging(bool needs) { }
    public virtual void StartCharging() { }

}
