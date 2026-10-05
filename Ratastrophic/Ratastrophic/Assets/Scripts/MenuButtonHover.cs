using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Arrow")]
    public GameObject arrow;

    [Header("Blink Settings")]
    public float blinkSpeed = 0.15f;

    private bool isHovering = false;
    private float blinkTimer = 0f;

    void Start()
    {
        // La flecha empieza oculta
        if (arrow != null)
        {
            arrow.SetActive(false);
        }
    }

    void Update()
    {
        if (!isHovering || arrow == null)
            return;

        blinkTimer += Time.deltaTime;

        if (blinkTimer >= blinkSpeed)
        {
            arrow.SetActive(!arrow.activeSelf);
            blinkTimer = 0f;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        blinkTimer = 0f;

        // Flecha aparece al pasar por encima
        if (arrow != null)
        {
            arrow.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        blinkTimer = 0f;

        // Flecha desaparece al salir
        if (arrow != null)
        {
            arrow.SetActive(false);
        }
    }
}