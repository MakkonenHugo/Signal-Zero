using UnityEngine;
using UnityEngine.EventSystems;

public class PolaroidWeaponCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public RectTransform cardTransform;
    public float restAngle = -6f;
    public float hoverScale = 1.15f;
    public float restScale = 1f;
    public float hoverLiftY = 30f;
    public float animSpeed = 10f;

    private Vector2 restAnchoredPosition;
    private float targetAngle;
    private float targetScale;
    private float targetLiftY;
    private bool isHovered;

    public System.Action onSelected;

    private void Awake()
    {
        if (cardTransform == null)
            cardTransform = transform as RectTransform;

        restAnchoredPosition = cardTransform.anchoredPosition;
        cardTransform.localRotation = Quaternion.Euler(0f, 0f, restAngle);
        cardTransform.localScale = Vector3.one * restScale;

        targetAngle = restAngle;
        targetScale = restScale;
        targetLiftY = 0f;
    }

    private void Update()
    {
        float currentAngle = cardTransform.localEulerAngles.z;
        if (currentAngle > 180f)
            currentAngle -= 360f;

        float newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.unscaledDeltaTime * animSpeed);
        cardTransform.localRotation = Quaternion.Euler(0f, 0f, newAngle);

        float newScale = Mathf.Lerp(cardTransform.localScale.x, targetScale, Time.unscaledDeltaTime * animSpeed);
        cardTransform.localScale = Vector3.one * newScale;

        float currentLiftY = cardTransform.anchoredPosition.y - restAnchoredPosition.y;
        float newLiftY = Mathf.Lerp(currentLiftY, targetLiftY, Time.unscaledDeltaTime * animSpeed);
        cardTransform.anchoredPosition = new Vector2(restAnchoredPosition.x, restAnchoredPosition.y + newLiftY);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        targetAngle = 0f;
        targetScale = hoverScale;
        targetLiftY = hoverLiftY;

        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        targetAngle = restAngle;
        targetScale = restScale;
        targetLiftY = 0f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onSelected?.Invoke();
    }
}