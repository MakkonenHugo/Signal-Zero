using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 3f;
    public string deathSceneName = "YouDied";

    private float currentHealth;
    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        SceneManager.LoadScene(deathSceneName);
    }
}