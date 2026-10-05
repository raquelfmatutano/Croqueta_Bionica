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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (state.NPC_currentState == RobotState.PERSECUCION){
        //     float step =  NPC_follow_speed * Time.deltaTime;
        //     transform.position = Vector3.MoveTowards(transform.position, player.position, step);
        // }
    }

    void OnTriggerEnter(Collider obj) {
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
