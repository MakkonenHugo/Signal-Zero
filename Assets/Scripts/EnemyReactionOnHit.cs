using UnityEngine;

public class EnemyReactionOnHit : MonoBehaviour
{
    public Enemy enemy;
    public Vector3 spawnPosition;
    [Range(0f, 1f)]
    public float chargeChance = 0.5f;

    private void Awake()
    {
        if (spawnPosition == Vector3.zero)
        {
            spawnPosition = transform.position;
        }
    }

    public void OnHitBy(Vector3 shooterPosition)
    {
        if (enemy == null)
            return;

        if (!enemy.HasNotReactedYet)
            return;

        bool willCharge = Random.value < chargeChance;

        if (willCharge)
        {
            enemy.BeginCharge(shooterPosition);
        }
        else
        {
            enemy.BeginRetreatToSpawn(spawnPosition);
        }
    }
}