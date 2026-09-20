using System.Collections.Generic;
using UnityEngine;

public class WaypointGraph : MonoBehaviour
{
    private List<Waypoint> waypoints = new List<Waypoint>();

    private void Awake()
    {
        waypoints.AddRange(
            FindObjectsByType<Waypoint>(FindObjectsSortMode.None)
        );
    }

    public Waypoint ObtenerWaypointAleatorio(Waypoint waypointActual)
    {
        if (waypoints.Count <= 1)
            return null;

        List<Waypoint> posibles = new List<Waypoint>();

        foreach (Waypoint waypoint in waypoints)
        {
            if (waypoint != null && waypoint != waypointActual)
            {
                posibles.Add(waypoint);
            }
        }

        if (posibles.Count == 0)
            return null;

        int indice = Random.Range(
            0,
            posibles.Count
        );

        return posibles[indice];
    }
}