using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float maxHealth = 1f;
    public EnemyDeathHandler deathHandler;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

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