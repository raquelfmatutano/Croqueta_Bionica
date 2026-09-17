using System.Collections.Generic;
using UnityEngine;

public class WaypointGraph : MonoBehaviour
{
    public List<Waypoint> waypoints = new List<Waypoint>();

    private void Awake()
    {
        waypoints.AddRange(
            FindObjectsByType<Waypoint>(FindObjectsSortMode.None)
        );
    }

    public Waypoint ObtenerWaypointAleatorio()
    {
        if (waypoints.Count == 0)
            return null;

        int indice = Random.Range(
            0,
            waypoints.Count
        );

        return waypoints[indice];
    }
}