
using UnityEngine;

public class NoiseObject : MonoBehaviour{
    public AudioClip impactSound;
    public float soundCooldown = 0.5f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    [Header("Ajustes del área de ruido")]
    public float noiseRadius = 7f;
    public float noiseDuration = 2f;

    [Header("Limpio/Sucio")]
    public bool clean = true;

    private AudioSource audioSource;
    private float nextSoundTime;
    private GameObject currentNoiseArea;

    private SphereCollider cleaningArea;

    private void Start(){
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision){

        TryPlaySound(collision);
    }

    private void OnCollisionStay(Collision collision){

        TryPlaySound(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryClean(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        TryClean(other.gameObject);
    }

    private bool TryClean(GameObject targetGameObject)
    {
        if (clean) 
            return false;
        
        NPC_state state = targetGameObject.GetComponent<NPC_state>();

        if (targetGameObject.CompareTag("NPC") & state != null)
        {
            print("limpiado");
            transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
            clean = true;

            if (cleaningArea != null)
            {
                Destroy(cleaningArea);
            }

            state.NPC_currentState.NoiseReached();

            return true;
        }

        return false;
    }

    private void TryPlaySound(Collision collision){
         if (!clean){
            return;
        }
        
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time < nextSoundTime)
            return;
        
       

        nextSoundTime = Time.time + soundCooldown;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(impactSound);

        CreateNoiseArea();
        CreateCleaningArea();
    }

    private void CreateNoiseArea(){

        // Destruir la esfera anterior si existe
        if (currentNoiseArea != null){
            Destroy(currentNoiseArea);
        }

        //Crear la nueva esfera
        currentNoiseArea = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        currentNoiseArea.GetComponent<MeshRenderer>().enabled = false;

        currentNoiseArea.name = "NoiseArea";
        currentNoiseArea.tag = "Noise";
        currentNoiseArea.transform.position = transform.position;
        currentNoiseArea.transform.localScale = Vector3.one * (noiseRadius * 2f);

        SphereCollider sphere = currentNoiseArea.GetComponent<SphereCollider>();
        sphere.isTrigger = true;

        Rigidbody rb = currentNoiseArea.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        Destroy(currentNoiseArea, noiseDuration);

        clean = false;
        transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
    }

    private void CreateCleaningArea() {
        // Destruir la esfera anterior si existe
        if (cleaningArea != null){
            Destroy(cleaningArea);
        }

       cleaningArea = gameObject.AddComponent<SphereCollider>() as SphereCollider;
       cleaningArea.radius = 1.5f;
        cleaningArea.isTrigger = true;

    }
}