using UnityEngine;
using UnityEngine.UI;

public class SoftLightPulse : MonoBehaviour
{
    [Header("Opacity")]
    [Range(0f, 1f)]
    public float minimumOpacity = 0.55f;

    [Range(0f, 1f)]
    public float maximumOpacity = 0.9f;

    [Header("Speed")]
    public float speed = 0.5f;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (image == null)
            return;

        float alpha = Mathf.Lerp(
            minimumOpacity,
            maximumOpacity,
            (Mathf.Sin(Time.time * speed) + 1f) / 2f
        );

        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}