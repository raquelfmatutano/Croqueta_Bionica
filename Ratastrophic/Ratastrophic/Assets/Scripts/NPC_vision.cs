using UnityEngine;

public class NPC_vision : MonoBehaviour
{

    public enum TipoNPC //cambio
    {
        Robot,
        Gato
    }

    public TipoNPC tipoNPC;//cambio
    public bool jugadorVistoDuranteInspeccion = false;//cambio

    public float NPC_follow_speed = 10f; //esto en algun momento habria que moverlo a un sitio mejor

    private Transform parent;
    private bool player_seen = false;
    private Transform player;

    public Collision vision_area;

    public NPC_state state;

    public LayerMask obstacleMask;
    public float rayOriginHeight = 0.5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool check_for_walls(Transform obj_seen) {
        Vector3 origin = transform.position + Vector3.up * rayOriginHeight;
        Vector3 forward = transform.forward;

        float detectionDistance = Vector3.Distance(obj_seen.position, transform.position);

        if (Physics.Raycast(origin, forward, out RaycastHit hitCenter, detectionDistance, obstacleMask))
        {
            print("Hay un obstaculo en medio. No veo nada");
            return true;
        }

        return false;
    }

    void OnTriggerEnter(Collider obj) {
        if(check_for_walls(obj.transform)) return;

        if(obj.tag == "Player") {
            print("Player seen!.");
            player = obj.transform;
            
            //cambio
            if (tipoNPC == TipoNPC.Robot)
            {
                state.NPC_currentState = RobotState.PERSECUCION;
            }
            else if (tipoNPC == TipoNPC.Gato)
            {
                jugadorVistoDuranteInspeccion = true;
            }
            
        }
    }

    void OnTriggerExit(Collider obj) {
        //if(obj.tag == "Player" && state.NPC_currentState == RobotState.PERSECUCION) {
        //    print("Player lost.");
        //    player = null;

        //    state.NPC_currentState = RobotState.PATRULLA;
        //}

        if (obj.CompareTag("Player")) //cambio
        {
            player = null;
            player_seen = false;

            if (tipoNPC == TipoNPC.Robot &&
                state != null &&
                state.NPC_currentState == RobotState.PERSECUCION)
            {

                print("Player lost.");
                state.NPC_currentState = RobotState.PATRULLA;
            }
        }
    }

    public void IniciarInspeccion()//cambio
    {
        jugadorVistoDuranteInspeccion = false;
    }

    public bool PuedeColocarCartel()//cambio
    {
        return !jugadorVistoDuranteInspeccion;
    }
}
