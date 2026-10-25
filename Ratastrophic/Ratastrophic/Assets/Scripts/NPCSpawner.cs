using System.Collections;
using UnityEngine;
using TMPro;

public class NPCSpawner : MonoBehaviour
{
    
    public float tiempoInicial = 10f;
    public float intervaloSalida = 1.5f;
    public TMP_Text textoTemporizador;

    private NPCPathfinding[] npcs;

    private void Awake()
    {
        npcs = FindObjectsByType<NPCPathfinding>(
            FindObjectsSortMode.None
        );
    }

    private void Start()
    {
        StartCoroutine(ActivarNPCs());
    }

    private IEnumerator ActivarNPCs()
    {
        float tiempoRestante = tiempoInicial;
        MEOW_Pathfinding rumba = FindFirstObjectByType<MEOW_Pathfinding>();
        rumba.gameObject.SetActive(false);

        foreach (NPCPathfinding npc in npcs ) {
            npc.gameObject.SetActive(false);
        }

        textoTemporizador.gameObject.SetActive(true);

        while (tiempoRestante > 0)
        {
            if(textoTemporizador != null)
            {
                int segundos = Mathf.CeilToInt(tiempoRestante);
                int minutos = segundos / 60;
                int segundosRestantes = segundos % 60;
                textoTemporizador.text = minutos.ToString("00") + ":" + segundosRestantes.ToString("00");

            }
            tiempoRestante -= Time.deltaTime;
            yield return null;
        }
        textoTemporizador.gameObject.SetActive(false);

        foreach (NPCPathfinding npc in npcs)
        {
            if (npc != null)
            {
                npc.gameObject.SetActive(true);
                npc.ActivarMovimiento();
            }

            yield return new WaitForSeconds(intervaloSalida);
        }

        
        rumba.gameObject.SetActive(true);
        rumba.ActivarMovimiento();

        if(textoTemporizador != null)
        {
            textoTemporizador.gameObject.SetActive(false);
        }
    }
}