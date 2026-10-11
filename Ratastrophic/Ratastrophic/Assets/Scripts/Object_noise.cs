
using UnityEngine;

public class NoiseObject : MonoBehaviour
{
    public AudioClip impactSound;
    public float soundCooldown = 0.5f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    private AudioSource audioSource;
    private float nextSoundTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(impactSound);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        TryPlaySound(collision);
    }

    private void TryPlaySound(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time < nextSoundTime)
            return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(impactSound);
        
        nextSoundTime = Time.time + soundCooldown;
    }
}