using UnityEngine;

public class NPC_hearing : MonoBehaviour
{
    //public float NPC_speed = 5.0f; //esto en algun momento habria que moverlo a un sitio mejor

    public GameObject alert_area;

    private Transform parent;
    private bool noise_heard = false;
    private Vector3 noise_coords;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //parent = gameObject.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (noise_heard){
            float step =  NPC_speed * Time.deltaTime;
            parent.position = Vector3.MoveTowards(parent.position, noise_coords, step);
        }*/
    }

    public void ReceiveAlert()
    {
        NPC_state state = GetComponent<NPC_state>();
        if (state.NPC_currentState != RobotState.PATRULLA)
            return;
        

        print(name + ": I heard you, let's go!");
        state.NPC_currentState = RobotState.INVESTIGACION;
    }

    void OnTriggerEnter(Collider obj) {
        if(obj.tag == "Noise") {
            print("Noise detected");
            NPC_state state = GetComponent<NPC_state>();
            if (state.NPC_currentState != RobotState.PATRULLA)
                return;

            state.NPC_currentState = RobotState.INVESTIGACION;

            if (alert_area) {
                alert_area.SetActive(true);
                
            }
        

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
            alert_area.SetActive(false);
            foreach (var hit in hitColliders)
            {
                if (hit.CompareTag("NPC"))
                {
                    NPC_hearing otherNPC = hit.GetComponent<NPC_hearing>();
                    if (otherNPC != null && otherNPC != this)
                    {
                        print(name + ": Hey, do you hear me, " + hit.name + "?");
                        otherNPC.ReceiveAlert();
                        
                    }
                }
            }
            
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
