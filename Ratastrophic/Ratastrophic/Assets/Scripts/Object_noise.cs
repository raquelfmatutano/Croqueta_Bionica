using UnityEngine.VFX;
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
    public bool cleanable = true;
    public bool clean = true;

    [Header ("VFX")]
    public GameObject smokePoof;

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

    /*private void OnCollisionStay(Collision collision){

        TryPlaySound(collision);
    }*/

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
        /*if (clean) 
            return false;*/
        
        NPC_state state = targetGameObject.GetComponent<NPC_state>();

        if (targetGameObject.CompareTag("NPC") & state != null)
        {
            state.NPC_currentState.StartCleaning();
            clean = true;

            if (cleaningArea != null)
            {
                Destroy(cleaningArea);
            }

            return true;
        }

        return false;
    }

    private void StopToClean(GameObject obj) {
        NPC_state state = obj.GetComponent<NPC_state>();
        state.NPC_currentState.InterestSpotted(transform);
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
        if(cleanable) {
            PlayVFX();
            CreateCleaningArea();
        }
        
    }

    public void CleanObject() {
        print("limpiado");
        PlayVFX();
        transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
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
    }

    private void CreateCleaningArea() {
        // Destruir la esfera anterior si existe
        if (cleaningArea != null){
            Destroy(cleaningArea);
        }

        clean = false;
        transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);

       cleaningArea = gameObject.AddComponent<SphereCollider>() as SphereCollider;
       cleaningArea.radius = 0.8f;
       cleaningArea.isTrigger = true;

    }

    private void PlayVFX() {
        if (smokePoof != null) {
            GameObject vfxInstance = Instantiate(smokePoof, transform.position, Quaternion.identity, gameObject.transform);

            VisualEffect vfx = vfxInstance.GetComponent<VisualEffect>();
            if (vfx != null) {
                vfx.Play(); 
            }
            
            Destroy(vfxInstance, 1f);
        }
    }

    
}