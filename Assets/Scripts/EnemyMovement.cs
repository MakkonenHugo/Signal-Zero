using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float preferredDistance = 6f;
    public float distanceTolerance = 1.5f;
    public float rotationSpeed = 10f;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void ApproachOrRetreat(Transform target)
    {
        if (target == null || controller == null)
            return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float distance = toTarget.magnitude;

        Vector3 moveDirection = Vector3.zero;

        if (distance > preferredDistance + distanceTolerance)
        {
            moveDirection = toTarget.normalized;
        }
        else if (distance < preferredDistance - distanceTolerance)
        {
            moveDirection = -toTarget.normalized;
        }

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        if (toTarget.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void MoveTo(Vector3 point)
    {
        if (controller == null)
            return;

        Vector3 toPoint = point - transform.position;
        toPoint.y = 0f;

        if (toPoint.sqrMagnitude > 0.01f)
        {
            controller.Move(toPoint.normalized * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(toPoint.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void Stop()
    {
    }
}