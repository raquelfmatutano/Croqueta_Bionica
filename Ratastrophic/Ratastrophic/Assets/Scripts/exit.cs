using UnityEngine;

public class EscapeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // GetComponentInParent por si el collider del Player esta en un objeto hijo
            player_movemet player = other.GetComponentInParent<player_movemet>();

            if (player == null)
            {
                Debug.LogError("Error: El objeto con Tag 'Player' no tiene el script 'player_movemet'.");
                return;
            }

            if (player.hasPotion)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.TriggerWin();
                }
                else
                {
                    Debug.LogError("Error: No existe un GameManager en la escena o no tiene el script asignado.");
                }
            }
            else
            {
                Debug.Log("Necesitas encontrar la pocion antes de salir.");
            }
        }
    }
}