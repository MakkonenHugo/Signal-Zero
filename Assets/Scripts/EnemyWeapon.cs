using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage = 1f;
    public float range = 20f;
    public float fireRate = 0.5f;
    public bool isAutomatic = true;

    public Transform firePoint;
    public Transform target;
    public MuzzleFlash muzzleFlash;
    public LayerMask obstacleMask;
    public BulletTrail bulletTrail;
    public HitEffect hitEffect;
    public AudioSource audioSource;
    public AudioClip fireSound;

    public CharacterController targetController;
    public float missChanceAtMaxSpeed = 0.6f;
    public float speedForMaxMissChance = 6f;
    public float missAngleSpread = 8f;

    private float nextFireTime;
    private Vector3 lastTargetPosition;
    private bool hasLastTargetPosition;
    private bool hasFiredThisEngagement;

    private void OnEnable()
    {
        nextFireTime = Time.time + fireRate;
        hasLastTargetPosition = false;
        hasFiredThisEngagement = false;
    }

    private void OnDisable()
    {
        hasFiredThisEngagement = false;
    }

    private void Update()
    {
        if (target == null || firePoint == null)
            return;

        bool wantsToFire = isAutomatic || !hasFiredThisEngagement;

        if (wantsToFire && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
            hasFiredThisEngagement = true;
        }
    }

    private float GetTargetSpeed()
    {
        if (targetController != null)
        {
            Vector3 velocity = targetController.velocity;
            velocity.y = 0f;
            return velocity.magnitude;
        }

        if (target == null)
            return 0f;

        Vector3 currentPosition = target.position;

        if (!hasLastTargetPosition)
        {
            lastTargetPosition = currentPosition;
            hasLastTargetPosition = true;
            return 0f;
        }

        Vector3 delta = currentPosition - lastTargetPosition;
        delta.y = 0f;
        lastTargetPosition = currentPosition;

        return delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
    }

    private void Shoot()
    {
        Vector3 direction = target.position - firePoint.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (audioSource != null && fireSound != null)
            audioSource.PlayOneShot(fireSound);

        if (Physics.Linecast(transform.position, firePoint.position, obstacleMask))
            return;

        float targetSpeed = GetTargetSpeed();
        float speedFactor = Mathf.Clamp01(targetSpeed / speedForMaxMissChance);
        float missChance = speedFactor * missChanceAtMaxSpeed;
        bool willMiss = Random.value < missChance;

        if (willMiss)
        {
            float randomAngle = Random.Range(-missAngleSpread, missAngleSpread);
            direction = Quaternion.Euler(0f, randomAngle, 0f) * direction;
        }

        RaycastHit[] hits = Physics.RaycastAll(firePoint.position, direction, range);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        Vector3 trailEnd = firePoint.position + direction * range;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform.root))
                continue;

            trailEnd = hit.point;

            Damageable damageable = hit.collider.GetComponentInParent<Damageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            PlayerHealth playerHealth = hit.collider.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            if (hitEffect != null)
            {
                hitEffect.Play(hit.point, hit.normal);
            }

            break;
        }

        if (bulletTrail != null)
        {
            bulletTrail.Play(firePoint.position, trailEnd);
        }
    }
}