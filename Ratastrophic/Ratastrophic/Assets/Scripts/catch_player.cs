using UnityEngine;

public class catch_player : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
            else
            {
                Debug.LogError("Error: No existe un GameManager en la escena o no tiene el script asignado.");
            }
        }
    }
}