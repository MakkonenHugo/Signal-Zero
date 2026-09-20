using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.05f;
    public float smoothSpeed = 12f;

    private Vector3 normalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        normalScale = transform.localScale;
        targetScale = normalScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            smoothSpeed * Time.deltaTime
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = normalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = normalScale;
    }
}