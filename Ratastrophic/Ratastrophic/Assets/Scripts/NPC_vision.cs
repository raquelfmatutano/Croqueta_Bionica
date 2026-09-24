using UnityEngine;

public class NPC_vision : MonoBehaviour
{
    public float NPC_follow_speed = 10f; //esto en algun momento habria que moverlo a un sitio mejor

    private Transform parent;
    private bool player_seen = false;
    private Transform player;

    public Collision vision_area;

    public NPC_state state;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //parent = gameObject.transform.parent;
        print(state);
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
            
            state.NPC_currentState = RobotState.PERSECUCION;
        }
    }
}
