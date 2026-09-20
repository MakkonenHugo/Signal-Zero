using UnityEngine;
using System.Collections.Generic;

public class EnemyDialogueNotifier : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public List<DialogueLine> lines = new List<DialogueLine>();

    private bool hasTriggered;

    public void TriggerDialogue()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

        StartCoroutine(dialogueUI.PlayLines(lines));
    }
}