using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
    public Animator animator;
    public string fallTrigger = "Fall";
    public CharacterController controller;
    public Collider bodyCollider;

    public Enemy enemy;
    public EnemyVision vision;
    public EnemyMovement movement;
    public EnemyWeapon weapon;
    public EnemyDeactivator deactivator;

    private bool isDead;

    public bool IsDead => isDead;

    public void Die()
    {
        Debug.Log("EnemyDeathHandler.Die() called on " + gameObject.name);

        if (isDead)
            return;

        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger(fallTrigger);
        }

        if (controller != null)
        {
            controller.enabled = false;
        }

        if (bodyCollider != null)
        {
            bodyCollider.isTrigger = true;
        }

        if (enemy != null)
            enemy.enabled = false;

        if (vision != null)
            vision.enabled = false;

        if (movement != null)
            movement.enabled = false;

        if (weapon != null)
            weapon.enabled = false;

        if (deactivator != null)
            deactivator.enabled = false;
    }
}