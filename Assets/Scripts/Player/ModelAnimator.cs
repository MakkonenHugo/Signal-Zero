using UnityEngine;

public class ModelAnimator : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;

    public float bobAmount = 0.05f;
    public float bobSpeed = 8f;
    public float movingThreshold = 0.1f;

    private Vector3 basePosition;
    private float bobTimer;
    private Vector3 lastPosition;

    private void Start()
    {
        basePosition = transform.localPosition;
        lastPosition = controller != null ? controller.transform.position : transform.position;
    }

    private void Update()
    {
        if (controller == null)
            return;

        Vector3 currentPosition = controller.transform.position;
        Vector3 movement = currentPosition - lastPosition;
        movement.y = 0f;
        float speed = Time.deltaTime > 0f ? movement.magnitude / Time.deltaTime : 0f;
        lastPosition = currentPosition;

        float speedFactor = Mathf.Clamp01(speed / 4f);

        if (speedFactor > 0.05f)
        {
            bobTimer += Time.deltaTime * bobSpeed * speedFactor;
        }
        else
        {
            bobTimer = 0f;
        }

        float bobOffset = Mathf.Sin(bobTimer) * bobAmount * speedFactor;
        transform.localPosition = basePosition + new Vector3(0f, bobOffset, 0f);

        if (animator != null)
        {
            bool isMoving = speed > movingThreshold;
            animator.SetBool("IsMoving", isMoving);
            animator.speed = isMoving ? 1f : 0f;
        }
    }
}