using UnityEngine;

public class NPC_hearing : MonoBehaviour
{
    public GameObject alert_area;
    public NPC_state state;

    private Transform parent;
    private bool noise_heard = false;
    private Vector3 noise_coords;

    private WaypointGraph grafo; 
    public NPCPathfinding pathfinder;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grafo = FindFirstObjectByType<WaypointGraph>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReceiveAlert(Waypoint objective)
    {
        print(name + ": I heard you, let's go!");
        state.NPC_currentState.NoiseHeard(objective);
    }

    void OnTriggerEnter(Collider obj) {
        if (obj.CompareTag("Noise")) 
        {
            print("Noise detected");

            Waypoint objective = grafo.find_nearest_waypoint(obj.transform.position);
            state.NPC_currentState.NoiseHeard(objective);

            //Avisar a otros NPCs
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
            foreach (var hit in hitColliders)
            {
                if (hit.CompareTag("NPC"))
                {
                    NPC_hearing otherNPC = hit.GetComponent<NPC_hearing>();
                    if (otherNPC != null && otherNPC != this)
                    {
                        otherNPC.ReceiveAlert(objective);
                    }
                }
            }
        }
    }

    
}
