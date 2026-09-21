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

    public Waypoint createWaypoint(Vector3 pos) {
        GameObject newWaypoint = new GameObject();
        newWaypoint.transform.position = new Vector3(pos.x, 0, pos.z);
        newWaypoint.name = "Go_to";
        newWaypoint.AddComponent<Waypoint>();
        
        Waypoint newWaypoint_component = newWaypoint.GetComponent<Waypoint>();


        
        newWaypoint_component.vecinos = waypoints; //vecinos
        print(newWaypoint_component.vecinos);

        waypoints.Add(newWaypoint_component);

        return newWaypoint_component;

    }
}