
using UnityEngine;

public class NoiseObject : MonoBehaviour{
    public AudioClip impactSound;
    public float soundCooldown = 0.5f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    [Header("Ajustes del área de ruido")]
    public float noiseRadius = 7f;
    public float noiseDuration = 2f;

    private AudioSource audioSource;
    private float nextSoundTime;
    private GameObject currentNoiseArea;

    private void Start(){
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision){

        TryPlaySound(collision);
    }

    private void OnCollisionStay(Collision collision){

        TryPlaySound(collision);
    }

    private void TryPlaySound(Collision collision){
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time < nextSoundTime)
            return;

        nextSoundTime = Time.time + soundCooldown;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(impactSound);

        CreateNoiseArea();
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
}