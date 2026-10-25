using UnityEngine;

public class EscapeZone : MonoBehaviour
{
    public GameObject puertaIzq;
    public GameObject puertaDer;

    public void openDoor() 
    {
        //puertaDer.transform.position = new Vector3(1.359f, puertaDer.transform.position.y, puertaDer.transform.position.z);
        //puertaIzq.transform.global_position = new Vector3(2f, puertaIzq.transform.position.y, puertaIzq.transform.position.z);

        puertaIzq.SetActive(false);
        puertaDer.SetActive(false);
    }

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