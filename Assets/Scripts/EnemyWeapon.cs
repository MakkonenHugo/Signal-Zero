using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public int damage = 1;
    public float range = 20f;
    public float fireRate = 0.5f;

    public Transform firePoint;
    public Transform target;
    public MuzzleFlash muzzleFlash;
    public LayerMask obstacleMask;
    public BulletTrail bulletTrail;
    public HitEffect hitEffect;
    public AudioSource audioSource;
    public AudioClip fireSound;

    private float nextFireTime;

    private void OnEnable()
    {
        nextFireTime = Time.time + fireRate;
    }

    private void Update()
    {
        if (target == null || firePoint == null)
            return;

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
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