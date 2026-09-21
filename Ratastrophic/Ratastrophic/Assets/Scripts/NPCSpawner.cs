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
        
        foreach (NPCPathfinding npc in npcs)
        {
            if (npc != null)
            {
                npc.ActivarMovimiento();
            }

            yield return new WaitForSeconds(intervaloSalida);
        }

        if(textoTemporizador != null)
        {
            textoTemporizador.gameObject.SetActive(false);
        }
    }
}