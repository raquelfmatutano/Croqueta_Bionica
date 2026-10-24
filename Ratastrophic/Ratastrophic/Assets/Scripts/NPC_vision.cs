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

    public Collider vision_area;
    private bool visionActiva = true;

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
        if(state != null) {
            if(!state.activeVision && visionActiva) {
                DesactivarVision();
            }
            else {
                if(!visionActiva)
                    ActivarVision();
            }
        }
        
    }

    private bool check_for_walls(Transform obj_seen) {
        Vector3 origin = transform.position + Vector3.up * rayOriginHeight;
        Vector3 targetPosition = obj_seen.position + Vector3.up * 1.0f;
        Vector3 forward = transform.forward;

        Vector3 directionToPlayer = targetPosition - origin;

        float detectionDistance = Vector3.Distance(targetPosition, transform.position);
        directionToPlayer.Normalize();

        Debug.DrawRay(origin, directionToPlayer * detectionDistance, Color.red, 1.0f);
        

        if (Physics.Raycast(origin, directionToPlayer, out RaycastHit hitCenter, detectionDistance, obstacleMask))
        {
            print("Hay un obstaculo en medio. No veo nada");
            return true;
        }

        print("no he visto nada");

        return false;
    }

    void OnTriggerEnter(Collider obj) {

        if (tipoNPC == TipoNPC.Gato && !visionActiva)
            return;

        if (obj.tag == "Player") {
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

        else if (obj.GetComponent<NoiseObject>()) {
            NoiseObject obj_noise = obj.GetComponent<NoiseObject>();

            if(!obj_noise.clean) {
                state.NPC_currentState.InterestSpotted(obj.transform);
                print("ewewewew");
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

    public void ActivarVision()
    {
        visionActiva = true;

        if (vision_area != null)
        {
            vision_area.enabled = true;

            Renderer rend = vision_area.GetComponent<Renderer>();
            if (rend != null)
                rend.enabled = true;
        }
    }

    public void DesactivarVision()
    {
        //if (tipoNPC != TipoNPC.Gato)
        //    return;

        visionActiva = false;

        if (vision_area != null)
        {
            vision_area.enabled = false;

            Renderer rend = vision_area.GetComponent<Renderer>();
            if (rend != null)
                rend.enabled = false;
        }
    }
}
