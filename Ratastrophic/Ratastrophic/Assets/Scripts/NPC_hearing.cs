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
        
        if (state.NPC_currentState != RobotState.PATRULLA)
            return;
        

        print(name + ": I heard you, let's go!");
        state.NPC_currentState = RobotState.INVESTIGACION;

        Waypoint start = grafo.find_nearest_waypoint(transform.position);

        pathfinder.IrADestino(objective, start);
    }

    void OnTriggerEnter(Collider obj) {
        if(obj.tag == "Noise") {
            print("Noise detected");
            if (state.NPC_currentState != RobotState.PATRULLA)
                return;

            state.NPC_currentState = RobotState.INVESTIGACION;

            //encontrar waypoint mas cercano al sonido
            Waypoint objective = grafo.find_nearest_waypoint(obj.transform.position);
            Waypoint start = grafo.find_nearest_waypoint(transform.position);

            if (alert_area) {
                alert_area.SetActive(true);
                print("Area activated");
            }
        

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
            
            foreach (var hit in hitColliders)
            {
                print ("I found something");
                if (hit.CompareTag("NPC"))
                {
                    NPC_hearing otherNPC = hit.GetComponent<NPC_hearing>();
                    if (otherNPC != null && otherNPC != this)
                    {
                        print(name + ": Hey, do you hear me, " + hit.name + "?");
                        otherNPC.ReceiveAlert(objective);
                        
                    }
                }
            }

            alert_area.SetActive(false);

            pathfinder.IrADestino(objective, start);

            /*Waypoint start = grafo.createWaypoint(transform.position, pathfinder.objetivo, "start");
            Waypoint objective = grafo.createWaypoint(obj.transform.position, start, "objective");

            start.vecinos.Add(objective);
            pathfinder.IrADestino(objective, start);

            grafo.deleteWaypoints();*/
            
        }
    }

    /*private void OnTriggerStay(Collider obj)
    {
        if (obj.tag == "Alert")
        {
            print(name + ": I heard you, let's go");
        }
    }*/

    
}
