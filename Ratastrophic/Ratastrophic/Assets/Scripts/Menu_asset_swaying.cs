using UnityEngine;

public class AssetSway : MonoBehaviour
{
    [Header("Rotation")]
    public float angle = 2f;

    [Header("Speed")]
    public float speed = 0.5f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {
        float rotation = Mathf.Sin(Time.time * speed) * angle;

        transform.rotation = startRotation * Quaternion.Euler(0f, 0f, rotation);
    }
}