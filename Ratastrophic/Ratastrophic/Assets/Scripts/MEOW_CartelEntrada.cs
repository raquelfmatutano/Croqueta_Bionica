using UnityEngine;
using System.Collections.Generic;

public class MEOW_CartelEntrada : MonoBehaviour
{
    //IDs de las salas que comparten esta entrada
    public List<int> idsSalasConectadas = new List<int>();

    public bool YaColocado { get; private set; } = false;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public bool PerteneceASala(int idSala)
    {
        return idsSalasConectadas.Contains(idSala);
    }

    public void ColocarCartel()
    {
        YaColocado = true;
        gameObject.SetActive(true);
        Debug.Log("Cartel activado en " + transform.position);
    }
}
