using UnityEngine;

public class EscapeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_movemet player = other.GetComponent<player_movemet>();

            // Comprueba si el jugador tiene la pocion
            if (player != null && player.hasPotion)
            {
                GameManager.Instance.TriggerWin();
            }
            else
            {
                Debug.Log("Necesitas encontrar la pocion antes de salir.");
            }
        }
    }
}