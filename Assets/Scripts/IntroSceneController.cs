using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntroSceneController : MonoBehaviour
{
    [Header("Fade")]
    public UnityEngine.UI.Image blackOverlay;
    public float fadeInDuration = 3f;

    [Header("Dialogue")]
    public DialogueUI dialogueUI;
    public List<DialogueLine> lines = new List<DialogueLine>
    {
        new DialogueLine { speaker = "ROBOT", text = "Citizen Gary Knox." },
        new DialogueLine { speaker = "GARY", text = "Yeah?" },
        new DialogueLine { speaker = "ROBOT", text = "You are being relocated for your protection." },
        new DialogueLine { speaker = "GARY", text = "I'm not going anywhere." }
    };

    [Header("Objective")]
    public GameObject objectiveText;
    public float objectiveFlashSpeed = 2f;

    [Header("Tutorial Hint")]
    public GameObject tutorialHintPanel;
    public TMPro.TMP_Text tutorialHintText;
    public string tutorialHintMessage = "WASD to move, mouse to aim, hold right click for OVERRIDE, left click to shoot";
    public float tutorialHintDuration = 4f;

    [Header("Gameplay")]
    public GameObject playerWeapon;
    public GaryModelSwitcher modelSwitcher;
    public Enemy targetEnemy;
    public Damageable targetRobot;

    private bool robotDefeated;

    private void Start()
    {
        Time.timeScale = 0f;

        if (objectiveText != null)
            objectiveText.SetActive(false);

        if (tutorialHintPanel != null)
            tutorialHintPanel.SetActive(false);

        if (playerWeapon != null)
            playerWeapon.SetActive(false);

        if (targetEnemy != null)
            targetEnemy.combatEnabled = false;

        StartCoroutine(RunIntro());
    }

    private IEnumerator RunIntro()
    {
        if (blackOverlay != null)
        {
            yield return StartCoroutine(FadeImage(blackOverlay, 1f, 0f, fadeInDuration));
        }

        if (dialogueUI != null)
        {
            yield return StartCoroutine(dialogueUI.PlayLines(lines));
        }

        if (objectiveText != null)
        {
            objectiveText.SetActive(true);
            StartCoroutine(FlashObjective());
        }

        if (tutorialHintPanel != null)
        {
            tutorialHintText.text = tutorialHintMessage;
            tutorialHintPanel.SetActive(true);
            StartCoroutine(HideTutorialHintAfterDelay());
        }

        if (playerWeapon != null)
            playerWeapon.SetActive(true);

        if (modelSwitcher != null)
            modelSwitcher.SwitchToArmed();

        if (targetEnemy != null)
            targetEnemy.combatEnabled = true;

        Time.timeScale = 1f;

        if (targetRobot != null)
        {
            yield return new WaitUntil(() => robotDefeated);
        }

        if (objectiveText != null)
            objectiveText.SetActive(false);
    }

    private IEnumerator FlashObjective()
    {
        CanvasGroup group = objectiveText.GetComponent<CanvasGroup>();

        if (group == null)
            group = objectiveText.AddComponent<CanvasGroup>();

        while (objectiveText.activeSelf)
        {
            float t = (Mathf.Sin(Time.unscaledTime * objectiveFlashSpeed) + 1f) * 0.5f;
            group.alpha = Mathf.Lerp(0.5f, 1f, t);
            yield return null;
        }
    }

    private IEnumerator HideTutorialHintAfterDelay()
    {
        yield return new WaitForSecondsRealtime(tutorialHintDuration);

        if (tutorialHintPanel != null)
            tutorialHintPanel.SetActive(false);
    }

    public void NotifyRobotDefeated()
    {
        robotDefeated = true;
    }

    private IEnumerator FadeImage(UnityEngine.UI.Image image, float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = t * t;

            Color c = image.color;
            c.a = Mathf.Lerp(from, to, eased);
            image.color = c;

            yield return null;
        }

        Color finalColor = image.color;
        finalColor.a = to;
        image.color = finalColor;
    }
}