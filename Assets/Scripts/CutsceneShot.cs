using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CutsceneShot : MonoBehaviour
{
    public string nextSceneName = "Level6_Containment";
    public string playerTag = "Player";

    public Transform player;
    public Transform shooter;
    public Transform firePoint;
    public MonoBehaviour[] scriptsToDisable;

    public MuzzleFlash muzzleFlash;
    public BulletTrail bulletTrail;
    public AudioSource audioSource;
    public AudioClip fireSound;
    public CameraFollow cameraFollow;
    public float cameraKick = 0.4f;

    public Animator playerAnimator;
    public string fallTrigger = "Fall";

    public Image blackoutImage;
    public AudioSource blackoutAudioSource;
    public AudioClip blackoutSound;
    public float blackoutDelay = 0.05f;
    public float blackoutHoldDuration = 2f;

    public RectTransform panelTransform;
    public float panelSlideDuration = 0.6f;
    public float panelStartOffscreenX = 1600f;
    public float panelOnscreenX = 0f;

    public float startDelay = 0.8f;
    public float turnDuration = 0.6f;
    public float aimPause = 0.7f;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.transform.root.CompareTag(playerTag))
            return;

        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError("CutsceneShot: scene '" + nextSceneName + "' is not in Build Settings.");
            return;
        }

        triggered = true;
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        for (int i = 0; i < scriptsToDisable.Length; i++)
        {
            if (scriptsToDisable[i] != null)
                scriptsToDisable[i].enabled = false;
        }

        yield return new WaitForSeconds(startDelay);

        if (shooter != null && player != null)
        {
            Quaternion startRotation = shooter.rotation;
            Vector3 toPlayer = player.position - shooter.position;
            toPlayer.y = 0f;

            if (toPlayer.sqrMagnitude > 0.01f)
            {
                Quaternion endRotation = Quaternion.LookRotation(toPlayer.normalized);
                float elapsed = 0f;

                while (elapsed < turnDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / turnDuration);
                    t = t * t * (3f - 2f * t);
                    shooter.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                    yield return null;
                }

                shooter.rotation = endRotation;
            }
        }

        yield return new WaitForSeconds(aimPause);

        FireShot();

        if (playerAnimator != null)
            playerAnimator.SetTrigger(fallTrigger);

        yield return new WaitForSeconds(blackoutDelay);

        PlayBlackout();

        yield return new WaitForSeconds(blackoutHoldDuration);

        if (panelTransform != null)
            yield return StartCoroutine(SlidePanelIn());

        SceneManager.LoadScene(nextSceneName);
    }

    private void FireShot()
    {
        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (audioSource != null && fireSound != null)
            audioSource.PlayOneShot(fireSound);

        if (cameraFollow != null)
            cameraFollow.Kick(null, cameraKick);

        if (bulletTrail != null && firePoint != null && player != null)
            bulletTrail.Play(firePoint.position, player.position + Vector3.up);
    }

    private void PlayBlackout()
    {
        if (blackoutImage != null)
        {
            Color color = blackoutImage.color;
            color.a = 1f;
            blackoutImage.color = color;
            blackoutImage.gameObject.SetActive(true);
        }

        AudioSource source = blackoutAudioSource != null ? blackoutAudioSource : audioSource;

        if (source != null && blackoutSound != null)
            source.PlayOneShot(blackoutSound);
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