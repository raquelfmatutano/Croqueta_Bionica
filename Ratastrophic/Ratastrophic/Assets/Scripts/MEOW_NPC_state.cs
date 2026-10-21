using UnityEngine;

public class MEOW_NPC_state : NPC_state
{
    public MEOW_InspeccionSala inspeccionSala;
    public NPC_vision visionNPC;
    public MEOW_Pathfinding meowPathfinder;

    public new MEOW_Pathfinding pathfinder => meowPathfinder;

    public InspeccionarState inspeccionarState;
    public TrasladarState trasladarState;
    public AlertarState alertarState;

    public Waypoint waypointJugadorVisto;

    private void Awake()
    {
        if (meowPathfinder == null) meowPathfinder = GetComponent<MEOW_Pathfinding>();
        if (inspeccionSala == null) inspeccionSala = GetComponent<MEOW_InspeccionSala>();
        if (visionNPC == null) visionNPC = GetComponent<NPC_vision>();

        inspeccionarState = new InspeccionarState(this);
        trasladarState = new TrasladarState(this);
        alertarState = new AlertarState(this);
    }

    private void Start()
    {
        if (grafo == null)
            grafo = FindFirstObjectByType<WaypointGraph>();

        ChangeState(inspeccionarState);
    }

    private void Update()
    {
        if (visionNPC != null)
        {
            seesPlayer = visionNPC.jugadorVistoDuranteInspeccion;
        }

        NPC_currentState?.Update();
    }

    public new void ChangeState(State newState)
    {
        base.ChangeState(newState);
    }
}