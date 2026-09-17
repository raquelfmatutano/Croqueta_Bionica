using UnityEngine;

public class NPC_vision : MonoBehaviour
{
    public float NPC_speed = 5.0f; //esto en algun momento habria que moverlo a un sitio mejor

    private Transform parent;
    private bool player_seen = false;
    private Transform player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = gameObject.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (player_seen){
            float step =  NPC_speed * Time.deltaTime;
            parent.position = Vector3.MoveTowards(parent.position, player.gameObject.transform.position, step);
        }
    }

    void OnTriggerEnter(Collider obj) {
        if(obj.tag == "Player") {
            print("Player entered.");
                player = obj.transform;
                player_seen = true;
            }
    }
}
