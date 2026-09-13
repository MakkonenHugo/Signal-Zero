using UnityEngine;
using UnityEngine.UI;

public class OverrideScreenEffect : MonoBehaviour
{
    public OverrideController overrideController;
    public Image darkenOverlay;
    public float maxDarkenAlpha = 0.6f;
    public float fadeSpeed = 3f;

    private float currentAlpha;

    private void Update()
    {
        if (overrideController == null || darkenOverlay == null)
            return;

        float usedFraction = 1f - overrideController.MeterFraction;
        float targetAlpha = usedFraction * maxDarkenAlpha;

        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.unscaledDeltaTime * fadeSpeed);

        Color color = darkenOverlay.color;
        color.a = currentAlpha;
        darkenOverlay.color = color;
    }
}