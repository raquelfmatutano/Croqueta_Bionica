using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> vecinos = new List<Waypoint>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        foreach (Waypoint vecino in vecinos)
        {
            if (vecino != null)
            {
                Gizmos.DrawLine(
                    transform.position,
                    vecino.transform.position
                );
            }
        }

        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}