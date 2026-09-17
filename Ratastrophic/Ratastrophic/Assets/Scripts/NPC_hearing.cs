using UnityEngine;

public class NPC_hearing : MonoBehaviour
{
    //public float NPC_speed = 5.0f; //esto en algun momento habria que moverlo a un sitio mejor

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

    void OnTriggerEnter(Collider obj) {
        if(obj.tag == "Noise") {
            print("Noise detected");
        }
    }
}
