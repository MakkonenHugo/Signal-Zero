using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RespawnButton : MonoBehaviour
{
    public string fallbackSceneName = "Level1";

    public RectTransform panelTransform;
    public float panelSlideDuration = 0.6f;
    public float panelStartOffscreenX = 1600f;
    public float panelOnscreenX = 0f;

    private bool isRespawning;

    public void Respawn()
    {
        if (isRespawning)
            return;

        isRespawning = true;
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (panelTransform != null)
        {
            yield return StartCoroutine(SlidePanelIn());
        }

        Time.timeScale = 1f;

        string sceneToLoad = !string.IsNullOrEmpty(PlayerHealth.lastSceneName)
            ? PlayerHealth.lastSceneName
            : fallbackSceneName;

        if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            Debug.LogError("RespawnButton: scenea '" + sceneToLoad + "' ei loydy Build Settingsista.");
            isRespawning = false;
            yield break;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator SlidePanelIn()
    {
        float elapsed = 0f;
        Vector2 startPos = new Vector2(-panelStartOffscreenX, panelTransform.anchoredPosition.y);
        Vector2 endPos = new Vector2(panelOnscreenX, panelTransform.anchoredPosition.y);

        panelTransform.gameObject.SetActive(true);
        panelTransform.anchoredPosition = startPos;

        while (elapsed < panelSlideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / panelSlideDuration);
            t = t * t * (3f - 2f * t);

            panelTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        panelTransform.anchoredPosition = endPos;
    }
}