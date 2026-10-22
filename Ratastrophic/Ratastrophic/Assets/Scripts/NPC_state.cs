using UnityEngine;

public class NPC_state : MonoBehaviour
{
    public State NPC_currentState { get; private set; }

    public PatrullarState patrullarState;
    public InvestigarState investigarState;
    public PerseguirState perseguirState;
    public LimpiarState limpiarState;

    public bool seesPlayer;
    public bool hearsNoise;

    public Transform currentInterest;

    public NPCPathfinding pathfinder;
    public WaypointGraph grafo;

    private void Awake()
    {
        patrullarState = new PatrullarState(this);
        investigarState = new InvestigarState(this);
        perseguirState = new PerseguirState(this);
        limpiarState = new LimpiarState(this);
    }

    private void Start()
    {
        if (grafo == null)
            grafo = FindFirstObjectByType<WaypointGraph>();
            
        ChangeState(patrullarState);
    }

    private void Update()
    {
        //NPC_currentState?.Update();
    }

    public void ChangeState(State newState)
    {
        NPC_currentState?.Exit();
        NPC_currentState = newState;
        NPC_currentState.Enter();
    }
}