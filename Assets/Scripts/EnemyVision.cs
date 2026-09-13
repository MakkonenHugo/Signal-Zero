using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public float viewDistance = 12f;
    public float viewAngle = 90f;
    public LayerMask obstacleMask;
    public Transform eyePoint;

    public bool CanSeeTarget(Transform target)
    {
        if (target == null)
            return false;

        Vector3 origin = eyePoint != null ? eyePoint.position : transform.position;
        Vector3 toTarget = target.position - origin;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;

        if (distance > viewDistance)
            return false;

        float angle = Vector3.Angle(transform.forward, toTarget);

        if (angle > viewAngle * 0.5f)
            return false;

        if (Physics.Raycast(origin, toTarget.normalized, distance, obstacleMask))
        {
            return false;
        }

        return true;
    }
}