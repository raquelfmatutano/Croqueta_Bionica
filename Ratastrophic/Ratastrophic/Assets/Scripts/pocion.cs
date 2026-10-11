using UnityEngine;

public class PotionItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Notificamos al jugador que recogio la pocion
            player_movemet player = other.GetComponent<player_movemet>();
            if (player != null)
            {
                player.hasPotion = true;
                Destroy(gameObject);
            }
        }
    }
}