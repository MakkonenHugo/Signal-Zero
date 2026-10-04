using UnityEngine;
using System.Collections;

public class SignalZeroIntro : MonoBehaviour
{
    public RectTransform panelTransform;
    public float startDelay = 0.3f;
    public float holdDuration = 1.2f;
    public float slideOutDuration = 0.6f;
    public float slideOutOffscreenX = 1600f;

    private void Start()
    {
        if (panelTransform != null)
        {
            panelTransform.anchoredPosition = new Vector2(0f, panelTransform.anchoredPosition.y);
        }

        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        yield return new WaitForSecondsRealtime(holdDuration);

        if (panelTransform != null)
        {
            yield return StartCoroutine(SlidePanelOut());
        }

        gameObject.SetActive(false);
    }

    private IEnumerator SlidePanelOut()
    {
        float elapsed = 0f;
        Vector2 startPos = panelTransform.anchoredPosition;
        Vector2 endPos = new Vector2(slideOutOffscreenX, startPos.y);

        while (elapsed < slideOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / slideOutDuration);
            t = t * t * (3f - 2f * t);

            panelTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        panelTransform.anchoredPosition = endPos;
    }
}