using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class EnemyDeactivator : MonoBehaviour
{
    public Transform target;
    public Enemy enemy;
    public float deactivateRange = 2f;
    public EnemyDeathHandler deathHandler;
    public UnityEvent onDeactivated;

    public bool IsInRange => IsTargetInRange();
    public bool CanBeDeactivated => CanDeactivate();

    private bool ReadyForDeactivation => IsTargetInRange() && CanDeactivate();

    private void Update()
    {
        if (ReadyForDeactivation && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Deactivate();
        }
    }

    private bool IsTargetInRange()
    {
        if (target == null)
            return false;

        return Vector3.Distance(transform.position, target.position) <= deactivateRange;
    }

    private bool CanDeactivate()
    {
        return enemy == null || !enemy.IsAttacking;
    }

    private void Deactivate()
    {
        if (RouteTracker.Instance != null)
        {
            RouteTracker.Instance.RegisterDeactivation();
        }

        if (deathHandler != null)
        {
            deathHandler.Die();
        }
        else
        {
            Destroy(gameObject);
        }

        onDeactivated.Invoke();
    }
}