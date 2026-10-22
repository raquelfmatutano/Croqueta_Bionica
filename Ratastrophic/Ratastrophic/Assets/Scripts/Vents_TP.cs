using UnityEngine;

public class Vents_TP : MonoBehaviour{
    public Transform destino;
    public AudioClip sonidoTP;
    public float cooldown = 1.5f;

    [Header ("Opciones de sonido")]
    public float noiseRadius = 7f;
    public float noiseDuration = 2f;

    private AudioSource audioSource;
    private GameObject currentNoiseArea;
    private float siguienteUso = 0f;

    private void Start(){
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collision){

        if (!collision.CompareTag("Player"))
        return;

        if (Time.time < siguienteUso)
            return;

        collision.transform.position = destino.position;
        if (audioSource != null && sonidoTP != null){
            audioSource.PlayOneShot(sonidoTP);
        }
        CreateNoiseArea();

        // Bloquear la trampilla de destino por unos segundos
        Vents_TP trampillaDestino = destino.GetComponent<Vents_TP>();

        if (trampillaDestino != null){
            trampillaDestino.siguienteUso = Time.time + cooldown;
        }
    }

    private void CreateNoiseArea(){
        currentNoiseArea = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        currentNoiseArea.name = "NoiseArea";
        currentNoiseArea.tag = "Noise";
        currentNoiseArea.transform.position = transform.position;
        currentNoiseArea.transform.localScale = Vector3.one * (noiseRadius * 2f);

        currentNoiseArea.GetComponent<MeshRenderer>().enabled = false;

        SphereCollider sphere = currentNoiseArea.GetComponent<SphereCollider>();
        sphere.isTrigger = true;

        Rigidbody rb = currentNoiseArea.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        Destroy(currentNoiseArea, noiseDuration);
    }
}