using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
}

public class DialogueUI : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text speakerText;
    public TMP_Text dialogueText;
    public float typeSpeed = 0.03f;

    [Header("Typing Sound")]
    public AudioSource typingAudioSource;
    public AudioClip typingBlipSound;
    [Range(0f, 1f)] public float typingBlipVolume = 0.5f;
    public Vector2 pitchRandomRange = new Vector2(0.95f, 1.05f);
    public bool skipSoundOnWhitespace = true;

    private bool advanceRequested;
    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    public IEnumerator PlayLines(List<DialogueLine> lines)
    {
        isPlaying = true;

        float previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        foreach (DialogueLine line in lines)
        {
            yield return StartCoroutine(PlayLine(line));
            yield return StartCoroutine(WaitForAdvance());
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        Time.timeScale = previousTimeScale;

        isPlaying = false;
    }

    private IEnumerator PlayLine(DialogueLine line)
    {
        if (speakerText != null)
            speakerText.text = line.speaker;

        dialogueText.text = "";

        foreach (char letter in line.text)
        {
            dialogueText.text += letter;
            PlayTypingBlip(letter);
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
    }

    private void PlayTypingBlip(char letter)
    {
        if (typingAudioSource == null || typingBlipSound == null)
            return;

        if (skipSoundOnWhitespace && char.IsWhiteSpace(letter))
            return;

        typingAudioSource.pitch = Random.Range(pitchRandomRange.x, pitchRandomRange.y);
        typingAudioSource.PlayOneShot(typingBlipSound, typingBlipVolume);
    }

    private IEnumerator WaitForAdvance()
    {
        advanceRequested = false;

        yield return new WaitUntil(() => AdvancePressed());

        advanceRequested = false;
    }

    private bool AdvancePressed()
    {
        bool pressed = (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            || (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (pressed)
            advanceRequested = true;

        return advanceRequested;
    }
}