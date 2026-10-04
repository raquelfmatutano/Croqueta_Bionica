using UnityEngine;

public class MEOW_InspeccionSala : MonoBehaviour
{
    public NPC_vision visionNPC;
    public bool inspeccionEnCurso = false;

    public void ComenzarInspeccion()
    {
        inspeccionEnCurso = true;
        visionNPC.IniciarInspeccion();

        Debug.Log("Inspección iniciada");
    }

    public void TerminarInspeccion()
    {
        inspeccionEnCurso = false;

        if (visionNPC.PuedeColocarCartel())
        {
            Debug.Log("El jugador no ha sido visto. Se puede colocar el cartel.");
        }
        else
        {
            Debug.Log("El jugador ha sido visto. No se coloca el cartel.");
        }
    }
}
