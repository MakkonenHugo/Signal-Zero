using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public float duration = 0.05f;
    public float rotationSpeed = 500f;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Play()
    {
        gameObject.SetActive(true);

        transform.localRotation = Quaternion.Euler(
            0f,
            0f,
            Random.Range(0f, 360f)
        );

        CancelInvoke();
        Invoke(nameof(Hide), duration);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}