using UnityEngine;

public class NPC_ObjectDetection : MonoBehaviour
{
    public float detectionDistance = 3f;
    public float whiskerAngle = 30f;
    public float rayOriginHeight = 0.5f;
    public LayerMask obstacleMask;

    public float avoidanceWeight = 2.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 CalculateAvoidanceForce()
    {
        Vector3 origin = transform.position + Vector3.up * rayOriginHeight;
        Vector3 forward = transform.forward;
        Vector3 left = Quaternion.Euler(0, -whiskerAngle, 0) * forward;
        Vector3 right = Quaternion.Euler(0, whiskerAngle, 0) * forward;

        Vector3 avoidanceForce = Vector3.zero;

        // Rayo central
        if (Physics.Raycast(origin, forward, out RaycastHit hitCenter, detectionDistance, obstacleMask))
        {
            float urgency = 1f - (hitCenter.distance / detectionDistance);
            avoidanceForce += hitCenter.normal * urgency;
        }

        // Rayo izquierdo
        if (Physics.Raycast(origin, left, out RaycastHit hitLeft, detectionDistance, obstacleMask))
        {
            float urgency = 1f - (hitLeft.distance / detectionDistance);
            avoidanceForce += transform.right * urgency;
        }

        // Rayo derecho
        if (Physics.Raycast(origin, right, out RaycastHit hitRight, detectionDistance, obstacleMask))
        {
            float urgency = 1f - (hitRight.distance / detectionDistance);
            avoidanceForce -= transform.right * urgency;
        }

        avoidanceForce.y = 0f;

        if (avoidanceForce.sqrMagnitude > 0.001f)
        {
            return avoidanceForce.normalized * avoidanceWeight;
        }

        return Vector3.zero;
    }
}
