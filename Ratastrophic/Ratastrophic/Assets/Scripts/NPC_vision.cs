using UnityEngine;

public class NPC_vision : MonoBehaviour
{

    public enum TipoNPC //cambio
    {
        Robot,
        Gato
    }

    public TipoNPC tipoNPC;
    public bool jugadorVistoDuranteInspeccion = false;

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
        print("robot" + transform.position);
        print("origen ray" + origin);
        print("jugador" + obj_seen.transform.position);

        print("foward" + forward);

        if (Physics.Raycast(origin, forward, out RaycastHit hitCenter, detectionDistance, obstacleMask))
        {
            print("Hay un obstaculo en medio. No veo nada");
            return true;
        }

        print("no he visto nada");

        return false;
    }

    void OnTriggerEnter(Collider obj) {
        if(obj.tag == "Player") {
            if(check_for_walls(obj.transform)) 
                return;

            print("Player seen!.");
            player = obj.transform;
            
            if (tipoNPC == TipoNPC.Robot)
            {
                state.NPC_currentState.PlayerSpotted();
            }
            else if (tipoNPC == TipoNPC.Gato)
            {
                jugadorVistoDuranteInspeccion = true;
            }
            
        }
    }

    void OnTriggerExit(Collider obj) {

        if (obj.CompareTag("Player"))
        {
            player = null;
            player_seen = false;

            if (tipoNPC == TipoNPC.Robot && state != null)
            {

                print("Player lost.");
                state.NPC_currentState.PlayerLost();
            }
        }
    }

    public void IniciarInspeccion()
    {
        jugadorVistoDuranteInspeccion = false;
    }

    public bool PuedeColocarCartel()
    {
        return !jugadorVistoDuranteInspeccion;
    }
}
