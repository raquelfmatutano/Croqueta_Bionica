
using System.Collections.Generic;
using UnityEngine;

public class GirarCartel : MonoBehaviour
{
    public Transform puertaPivot;
    public float anguloApertura = 90f;
    public float velocidad = 2f;

    private Quaternion rotacionCerrada;
    private Quaternion rotacionObjetivo;

    private HashSet<Collider> npcsDentro =
        new HashSet<Collider>();

    void Start()
    {
        rotacionCerrada = puertaPivot.localRotation;
        rotacionObjetivo = rotacionCerrada;
    }

    void Update()
    {
        puertaPivot.localRotation = Quaternion.Slerp(
            puertaPivot.localRotation,
            rotacionObjetivo,
            velocidad * Time.deltaTime
        );
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignorar al jugador
        if (other.CompareTag("Player"))
            return;

        // Solo aceptar el collider del objeto NPC principal
        if (other.attachedRigidbody == null ||
            other.attachedRigidbody.transform != other.transform)
            return;

        npcsDentro.Add(other);

        Vector3 direccionNPC =
            other.bounds.center - puertaPivot.position;

        float lado = Vector3.Dot(
            direccionNPC,
            puertaPivot.forward
        );

        float sentido = lado >= 0f ? 1f : -1f;

        rotacionObjetivo = rotacionCerrada *
            Quaternion.Euler(
                0f,
                sentido * anguloApertura,
                0f
            );
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            return;

        if (other.attachedRigidbody == null ||
            other.attachedRigidbody.transform != other.transform)
            return;

        npcsDentro.Remove(other);

        if (npcsDentro.Count == 0)
        {
            rotacionObjetivo = rotacionCerrada;
        }
    }

}
