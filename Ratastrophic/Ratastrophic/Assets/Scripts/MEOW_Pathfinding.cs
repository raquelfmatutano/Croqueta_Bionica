using UnityEngine;
using System.Collections.Generic;
using System;

public class MEOW_Pathfinding : MonoBehaviour
{
    public Waypoint waypointInicial;
    public Waypoint waypointActual;
    public Waypoint objetivo;
    public Transform waypointDestino;

    public float velocidad = 4f;
    public float fuerzaMaxima = 5f;
    public float radioLlegada = 4f;
    public float velocidadGiro = 5f;

    private Vector3 velocidadActual;

    private AStar aStar;
    private List<Waypoint> ruta;
    private int indiceRuta = 0;
    private bool puedeMoverse = false;
    public event Action<Waypoint> AlcanzoWaypoint;

    private NPC_ObjectDetection objectDetection;

    private void Awake()
    {
        aStar = GetComponent<AStar>();
        objectDetection = GetComponent<NPC_ObjectDetection>();

        if (aStar == null)
        {
            Debug.LogError("El gato necesita el componente AStar.");
        }
    }

    private void Start()
    {
        if (waypointInicial == null)
        {
            Debug.LogError("Debes asignar el waypoint inicial del gato.");
            return;
        }

        waypointActual = waypointInicial;

        Vector3 posicion = waypointActual.transform.position;
        posicion.y = transform.position.y;
        transform.position = posicion;
    }

    public void IrADestino(Waypoint nuevoObjetivo, Waypoint actual)
    {
        if (nuevoObjetivo == null || actual == null || aStar == null)
        {
            Debug.LogError("No se puede calcular la ruta del gato: falta una referencia.");
            return;
        }

        objetivo = nuevoObjetivo;
        waypointActual = actual;

        ruta = aStar.CalcularRuta(waypointActual, objetivo);

        if (ruta == null || ruta.Count == 0)
        {
            Debug.LogError("No se ha encontrado una ruta hasta " + objetivo.name);
            return;
        }

        // Si la ruta contiene el nodo actual en el �ndice 0, avanzamos directamente al siguiente (�ndice 1)
        indiceRuta = (ruta.Count > 1 && ruta[0] == waypointActual) ? 1 : 0;

        // Aseguramos que el movimiento est� habilitado
        puedeMoverse = true;

        //para ver la ruta
        string rutaCompleta = "Ruta calculada: ";
        Debug.Log("Destino del gato: " + objetivo.name +
          " (Sala " + objetivo.idSala + ")");

        for (int i = 0; i < ruta.Count; i++)
        {
            rutaCompleta += ruta[i].name +
                            " (Sala " + ruta[i].idSala + ")";

            if (i < ruta.Count - 1)
                rutaCompleta += " -> ";
        }

        Debug.Log(rutaCompleta);
        //

        indiceRuta = ruta.Count > 1 ? 1 : 0;
        Debug.Log(
                "Nueva ruta: inicio=" + waypointActual.name +
                " | destino=" + objetivo.name +
                " | siguiente=" + ruta[indiceRuta].name +
                " | indice=" + indiceRuta
            );
    }

    private void Update()
    {

        if (!puedeMoverse || ruta == null || ruta.Count == 0)
            return;

        if (indiceRuta >= ruta.Count)
            return;

        waypointDestino = ruta[indiceRuta].transform;

        float distanciaAlWaypoint = Vector3.Distance(transform.position,waypointDestino.position);

        if (distanciaAlWaypoint <= radioLlegada)
        {
            Waypoint alcanzado = ruta[indiceRuta];

            waypointActual = alcanzado;

            Debug.Log("Gato alcanza waypoint: " + alcanzado.name +
                      " | Sala: " + alcanzado.idSala);

            Debug.Log("Objetivo actual: " + objetivo.name);

            indiceRuta++;

            AlcanzoWaypoint?.Invoke(alcanzado);

            if (ruta == null || indiceRuta >= ruta.Count)
            {
                velocidadActual = Vector3.zero;
                return;
            }

            return;
        }

        //SEEK

        Vector3 haciaObjetivo = waypointDestino.position - transform.position;

        Vector3 velocidadDeseada =
            haciaObjetivo.sqrMagnitude > 0.001f
                ? haciaObjetivo.normalized * velocidad
                : Vector3.zero;

        //STEERING

        Vector3 fuerzaDireccion = velocidadDeseada - velocidadActual;

        if (objectDetection != null)
        {
            fuerzaDireccion +=
                objectDetection.CalculateAvoidanceForce() * fuerzaMaxima;
        }

        fuerzaDireccion = Vector3.ClampMagnitude(fuerzaDireccion, fuerzaMaxima);
        velocidadActual += fuerzaDireccion * Time.deltaTime;
        velocidadActual = Vector3.ClampMagnitude(velocidadActual, velocidad);

        transform.position += velocidadActual * Time.deltaTime;

        // Girar hacia la direccion real del movimiento.

        Vector3 direccionMovimiento = velocidadActual;
        direccionMovimiento.y = 0f;

        if (direccionMovimiento.sqrMagnitude > 0.001f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(
                direccionMovimiento,
                Vector3.up
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadGiro * Time.deltaTime
            );
        }
    }

    public void ActivarMovimiento()
    {
        print("movimiento gato activado");
        puedeMoverse = true;
    }

    public bool HaLlegadoADestino()
    {
        if (objetivo == null)
            return false;

        return Vector3.Distance(
            transform.position,
            objetivo.transform.position
        ) <= radioLlegada;
    }

    public void DetenerMovimiento()
    {
        puedeMoverse = false;
        velocidadActual = Vector3.zero;
    }
}
