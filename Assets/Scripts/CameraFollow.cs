using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothTime = 0.25f;

    public bool sway = true;
    public float swayAmount = 0.02f;
    public float swaySpeed = 0.15f;

    public float kickAmount = 0.15f;
    public float kickDuration = 0.08f;
    public float kickRecoverTime = 0.15f;

    public bool allowZoom = true;
    public float zoomSpeed = 2f;
    public float minZoomDistance = 4f;
    public float maxZoomDistance = 14f;
    public float zoomSmoothTime = 0.15f;

    private Vector3 followVelocity;
    private float swaySeedX;
    private float swaySeedY;
    private Vector3 kickOffset;
    private Vector3 kickVelocity;
    private float kickTimer;
    private Vector3 kickDirection;
    private float currentKickAmount;

    private Vector3 offsetDirection;
    private float currentZoomDistance;
    private float targetZoomDistance;
    private float zoomVelocity;

    private void Start()
    {
        swaySeedX = Random.Range(0f, 1000f);
        swaySeedY = swaySeedX + 500f;

        currentZoomDistance = offset.magnitude;
        targetZoomDistance = currentZoomDistance;
        offsetDirection = currentZoomDistance > 0.0001f ? offset / currentZoomDistance : Vector3.back;
    }

    private void Update()
    {
        if (!allowZoom || Mouse.current == null)
            return;

        float scrollValue = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scrollValue) > 0.01f)
        {
            targetZoomDistance -= scrollValue * zoomSpeed * 0.01f;
            targetZoomDistance = Mathf.Clamp(targetZoomDistance, minZoomDistance, maxZoomDistance);
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        currentZoomDistance = Mathf.SmoothDamp(currentZoomDistance, targetZoomDistance, ref zoomVelocity, zoomSmoothTime);
        Vector3 zoomedOffset = offsetDirection * currentZoomDistance;

        Vector3 desiredPosition = target.position + zoomedOffset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, smoothTime);

        Vector3 swayOffset = Vector3.zero;
        if (sway)
        {
            float swayX = (Mathf.PerlinNoise(swaySeedX, Time.time * swaySpeed) - 0.5f) * 2f;
            float swayY = (Mathf.PerlinNoise(swaySeedY, Time.time * swaySpeed) - 0.5f) * 2f;
            swayOffset = new Vector3(swayX, swayY, 0f) * swayAmount;
        }

        UpdateKick();

        transform.position = smoothedPosition + swayOffset + kickOffset;
    }

    private void UpdateKick()
    {
        if (kickTimer > 0f)
        {
            kickTimer -= Time.unscaledDeltaTime;
            Vector3 target = kickDirection * currentKickAmount;
            kickOffset = Vector3.Lerp(kickOffset, target, Time.unscaledDeltaTime / Mathf.Max(kickDuration, 0.001f));
        }
        else
        {
            kickOffset = Vector3.SmoothDamp(kickOffset, Vector3.zero, ref kickVelocity, kickRecoverTime, Mathf.Infinity, Time.unscaledDeltaTime);
        }
    }

    public void Kick(Vector3? direction = null, float? amountOverride = null)
    {
        kickTimer = kickDuration;
        kickDirection = direction.HasValue
            ? direction.Value.normalized
            : new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
        currentKickAmount = amountOverride ?? kickAmount;
    }
}