using UnityEngine;

public enum EnemyState
{
    Idle,
    Alert,
    Attacking,
    Searching,
    Charging,
    Retreating
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
    public float chargeArrivalDistance = 0.5f;
    public float retreatArrivalDistance = 0.5f;
    public bool combatEnabled = true;

    public bool IsAttacking => state == EnemyState.Attacking;

    public bool HasNotReactedYet => state == EnemyState.Idle || state == EnemyState.Alert;

    private EnemyState state = EnemyState.Idle;
    private float alertTimer;
    private float searchDelayTimer;
    private Vector3 lastKnownPosition;

    private Vector3 chargeTargetPosition;
    private Vector3 retreatTargetPosition;

    public void BeginCharge(Vector3 shooterPosition)
    {
        chargeTargetPosition = shooterPosition;
        lastKnownPosition = shooterPosition;
        state = EnemyState.Charging;
    }

    public void BeginRetreatToSpawn(Vector3 spawnPosition)
    {
        retreatTargetPosition = spawnPosition;
        state = EnemyState.Retreating;
    }

    private void Update()
    {
        bool canSee = combatEnabled && vision != null && vision.CanSeeTarget(target);

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

            case EnemyState.Charging:
                if (canSee)
                {
                    state = EnemyState.Attacking;
                    break;
                }

                float distanceToCharge = Vector3.Distance(transform.position, chargeTargetPosition);

                if (distanceToCharge <= chargeArrivalDistance)
                {
                    state = EnemyState.Searching;
                    searchDelayTimer = searchDelay;
                }
                break;

            case EnemyState.Retreating:
                if (canSee)
                {
                    state = EnemyState.Attacking;
                    break;
                }

                float distanceToSpawn = Vector3.Distance(transform.position, retreatTargetPosition);

                if (distanceToSpawn <= retreatArrivalDistance)
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
            else if (state == EnemyState.Charging)
            {
                movement.MoveTo(chargeTargetPosition);
            }
            else if (state == EnemyState.Retreating)
            {
                movement.MoveTowardsLookingAt(retreatTargetPosition, lastKnownPosition);
            }
            else
            {
                movement.Stop();
            }
        }
    }
}