using UnityEngine;

public enum EnemyState
{
    Idle,
    Alert,
    Attacking,
    Searching
}

public class Enemy : MonoBehaviour
{
    public Transform target;
    public EnemyVision vision;
    public EnemyMovement movement;
    public EnemyWeapon weapon;

    public float alertDuration = 0.3f;
    public float searchDelay = 1f;
    public float searchArrivalDistance = 0.5f;

    public bool IsAttacking => state == EnemyState.Attacking;

    private EnemyState state = EnemyState.Idle;
    private float alertTimer;
    private float searchDelayTimer;
    private Vector3 lastKnownPosition;

    private void Update()
    {
        bool canSee = vision != null && vision.CanSeeTarget(target);

        if (canSee && target != null)
        {
            lastKnownPosition = target.position;
        }

        switch (state)
        {
            case EnemyState.Idle:
                if (canSee)
                {
                    state = EnemyState.Alert;
                    alertTimer = alertDuration;
                }
                break;

            case EnemyState.Alert:
                if (!canSee)
                {
                    state = EnemyState.Idle;
                    break;
                }

                alertTimer -= Time.deltaTime;

                if (alertTimer <= 0f)
                {
                    state = EnemyState.Attacking;
                }
                break;

            case EnemyState.Attacking:
                if (!canSee)
                {
                    state = EnemyState.Searching;
                    searchDelayTimer = searchDelay;
                }
                break;

            case EnemyState.Searching:
                if (canSee)
                {
                    state = EnemyState.Attacking;
                    break;
                }

                if (searchDelayTimer > 0f)
                {
                    searchDelayTimer -= Time.deltaTime;
                    break;
                }

                float distanceToLastKnown = Vector3.Distance(transform.position, lastKnownPosition);

                if (distanceToLastKnown <= searchArrivalDistance)
                {
                    state = EnemyState.Idle;
                }
                break;
        }

        if (weapon != null)
        {
            weapon.enabled = state == EnemyState.Attacking && canSee;
        }

        if (movement != null)
        {
            if (canSee && (state == EnemyState.Attacking || state == EnemyState.Alert))
            {
                movement.ApproachOrRetreat(target);
            }
            else if (state == EnemyState.Searching && searchDelayTimer <= 0f)
            {
                movement.MoveTo(lastKnownPosition);
            }
            else
            {
                movement.Stop();
            }
        }
    }
}