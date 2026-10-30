using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WakeUpSequence : MonoBehaviour
{
    public Image blackoutImage;
    public MonoBehaviour[] scriptsToDisableDuringWakeUp;
    public GameObject[] objectsToHideDuringWakeUp;

    public Animator playerAnimator;
    public string wakeUpTrigger = "WakeUp";

    public AudioSource audioSource;
    public AudioClip heartbeatSound;

    public CameraFollow cameraFollow;
    public float blinkKick = 0.05f;

    public float startDelay = 1.5f;
    public float[] blinkPeaks = { 0.55f, 0.35f };
    public float blinkOpenDuration = 0.5f;
    public float blinkCloseDuration = 0.35f;
    public float blinkPause = 0.6f;
    public float finalOpenDuration = 1.8f;

    private void Start()
    {
        if (blackoutImage != null)
        {
            Color color = blackoutImage.color;
            color.a = 1f;
            blackoutImage.color = color;
            blackoutImage.gameObject.SetActive(true);
        }

        SetControlsEnabled(false);
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        if (audioSource != null && heartbeatSound != null)
            audioSource.PlayOneShot(heartbeatSound);

        yield return new WaitForSeconds(startDelay);

        for (int i = 0; i < blinkPeaks.Length; i++)
        {
            yield return StartCoroutine(FadeAlpha(1f, blinkPeaks[i], blinkOpenDuration));

            if (cameraFollow != null)
                cameraFollow.Kick(null, blinkKick);

            yield return new WaitForSeconds(blinkPause * 0.5f);
            yield return StartCoroutine(FadeAlpha(blinkPeaks[i], 1f, blinkCloseDuration));
            yield return new WaitForSeconds(blinkPause);
        }

        if (playerAnimator != null)
            playerAnimator.SetTrigger(wakeUpTrigger);

        yield return StartCoroutine(FadeAlpha(1f, 0f, finalOpenDuration));

        if (blackoutImage != null)
            blackoutImage.gameObject.SetActive(false);

        SetControlsEnabled(true);
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        if (blackoutImage == null)
            yield break;

        float elapsed = 0f;
        Color color = blackoutImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);
            color.a = Mathf.Lerp(from, to, t);
            blackoutImage.color = color;
            yield return null;
        }

        color.a = to;
        blackoutImage.color = color;
    }

    private void SetControlsEnabled(bool value)
    {
        for (int i = 0; i < scriptsToDisableDuringWakeUp.Length; i++)
        {
            if (scriptsToDisableDuringWakeUp[i] != null)
                scriptsToDisableDuringWakeUp[i].enabled = value;
        }

        for (int i = 0; i < objectsToHideDuringWakeUp.Length; i++)
        {
            if (objectsToHideDuringWakeUp[i] != null)
                objectsToHideDuringWakeUp[i].SetActive(value);
        }
    }
}