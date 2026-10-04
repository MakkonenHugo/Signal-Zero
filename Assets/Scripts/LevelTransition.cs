using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    public string targetSceneName = "Level2";
    public string playerTag = "Player";

    public bool useLostSignalScreen = false;
    public GameObject lostSignalScreen;
    public float delayBeforeScreen = 1.5f;
    public float delayBeforeLevel = 3f;

    public AudioSource transitionSound;

    public RectTransform panelTransform;
    public float panelSlideDuration = 0.6f;
    public float panelStartOffscreenX = 1600f;
    public float panelOnscreenX = 0f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.transform.root.CompareTag(playerTag))
            return;

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("LevelTransition: targetSceneName on tyhja objektilla " + gameObject.name);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError("LevelTransition: scenea '" + targetSceneName + "' ei loydy Build Settingsista.");
            return;
        }

        triggered = true;

        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        if (useLostSignalScreen)
        {
            yield return new WaitForSeconds(delayBeforeScreen);

            if (transitionSound != null)
            {
                transitionSound.Play();
            }

            if (lostSignalScreen != null)
            {
                lostSignalScreen.SetActive(true);
            }

            yield return new WaitForSeconds(delayBeforeLevel);
        }
        else if (transitionSound != null)
        {
            transitionSound.Play();
        }

        if (panelTransform != null)
        {
            yield return StartCoroutine(SlidePanelIn());
        }

        SceneManager.LoadScene(targetSceneName);
    }

    IEnumerator SlidePanelIn()
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