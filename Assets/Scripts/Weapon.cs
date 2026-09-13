using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    public float damage = 1f;
    public float range = 20f;
    public float fireRate = 0.25f;
    public bool isAutomatic = false;
    public float recoilKick = 0.15f;

    public Transform firePoint;
    public MuzzleFlash muzzleFlash;
    public CameraFollow cameraFollow;
    public BulletTrail bulletTrail;
    public HitEffect hitEffect;
    public AudioSource audioSource;
    public AudioClip fireSound;

    private float nextFireTime;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        bool wantsToFire = isAutomatic ? Mouse.current.leftButton.isPressed : Mouse.current.leftButton.wasPressedThisFrame;

        if (wantsToFire && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (firePoint == null)
            return;

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray cameraRay = mainCamera.ScreenPointToRay(mousePosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (!groundPlane.Raycast(cameraRay, out float distance))
            return;

        Vector3 targetPoint = cameraRay.GetPoint(distance);

        Vector3 direction = targetPoint - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (cameraFollow != null)
            cameraFollow.Kick(null, recoilKick);

        if (audioSource != null && fireSound != null)
            audioSource.PlayOneShot(fireSound);

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, range);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        Vector3 trailEnd = transform.position + direction * range;

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