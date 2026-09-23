using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float preferredDistance = 6f;
    public float distanceTolerance = 1.5f;
    public float rotationSpeed = 10f;

    public LayerMask obstacleMask;
    public float obstacleCheckDistance = 1f;
    public float avoidCapsuleRadius = 0.4f;
    public float avoidCapsuleHeight = 1.6f;
    public float avoidCommitTime = 0.6f;
    public float avoidClearCheckDistance = 2f;

    private CharacterController controller;

    private static readonly float[] AvoidAngles = { 0f, 30f, -30f, 60f, -60f, 90f, -90f };

    private bool isAvoiding;
    private float avoidAngle;
    private float avoidTimer;

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

        Vector3 desiredDirection = Vector3.zero;

        if (distance > preferredDistance + distanceTolerance)
        {
            desiredDirection = toTarget.normalized;
        }
        else if (distance < preferredDistance - distanceTolerance)
        {
            desiredDirection = -toTarget.normalized;
        }

        if (desiredDirection.sqrMagnitude > 0.01f)
        {
            Vector3 moveDirection = GetAvoidingDirection(desiredDirection);
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
        MoveTowardsLookingAt(point, point);
    }

    public void MoveTowardsLookingAt(Vector3 movePoint, Vector3 lookPoint)
    {
        if (controller == null)
            return;

        Vector3 toMovePoint = movePoint - transform.position;
        toMovePoint.y = 0f;

        if (toMovePoint.sqrMagnitude > 0.01f)
        {
            Vector3 moveDirection = GetAvoidingDirection(toMovePoint.normalized);
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        Vector3 toLookPoint = lookPoint - transform.position;
        toLookPoint.y = 0f;

        if (toLookPoint.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toLookPoint.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private Vector3 GetAvoidingDirection(Vector3 desiredDirection)
    {
        if (isAvoiding)
        {
            avoidTimer -= Time.deltaTime;

            Vector3 committedDirection = Quaternion.Euler(0f, avoidAngle, 0f) * desiredDirection;

            bool committedBlocked = IsBlocked(committedDirection, obstacleCheckDistance);
            bool desiredClear = !IsBlocked(desiredDirection, avoidClearCheckDistance);

            if (avoidTimer <= 0f && desiredClear)
            {
                isAvoiding = false;
            }
            else if (committedBlocked)
            {
                isAvoiding = false;
            }
            else
            {
                return committedDirection;
            }
        }

        if (!IsBlocked(desiredDirection, obstacleCheckDistance))
        {
            return desiredDirection;
        }

        for (int i = 1; i < AvoidAngles.Length; i++)
        {
            Vector3 candidateDirection = Quaternion.Euler(0f, AvoidAngles[i], 0f) * desiredDirection;

            if (!IsBlocked(candidateDirection, obstacleCheckDistance))
            {
                isAvoiding = true;
                avoidAngle = AvoidAngles[i];
                avoidTimer = avoidCommitTime;
                return candidateDirection;
            }
        }

        return Vector3.zero;
    }

    private bool IsBlocked(Vector3 direction, float checkDistance)
    {
        Vector3 point1 = transform.position + Vector3.up * (avoidCapsuleHeight - avoidCapsuleRadius);
        Vector3 point2 = transform.position + Vector3.up * avoidCapsuleRadius;

        return Physics.CapsuleCast(
            point1,
            point2,
            avoidCapsuleRadius,
            direction,
            checkDistance,
            obstacleMask
        );
    }

    public void Stop()
    {
    }
}