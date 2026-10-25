using UnityEngine;

public class PotionItem : MonoBehaviour
{
    public EscapeZone puerta;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_movemet player = other.GetComponent<player_movemet>();
            if (player != null)
            {
                player.hasPotion = true;
                puerta.openDoor();
                Destroy(gameObject);
            }
        }
    }
}