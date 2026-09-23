using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public float maxHealth = 1f;
    public EnemyDeathHandler deathHandler;
    public EnemyReactionOnHit reactionOnHit;
    public UnityEvent onDeath;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, null);
    }

    public void TakeDamage(float damage, Vector3? shooterPosition)
    {
        currentHealth -= damage;

        if (reactionOnHit != null && shooterPosition.HasValue)
        {
            reactionOnHit.OnHitBy(shooterPosition.Value);
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (RouteTracker.Instance != null)
        {
            RouteTracker.Instance.RegisterKill();
        }

        onDeath.Invoke();

        if (deathHandler != null)
        {
            deathHandler.Die();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}