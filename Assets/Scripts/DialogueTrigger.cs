using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class DialogueTrigger : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public List<DialogueLine> lines = new List<DialogueLine>();

    public float delayBeforeDialogue = 0f;

    private bool hasTriggered;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        if (hasTriggered)
            return;

        if (dialogueUI == null)
            return;

        hasTriggered = true;
        StartCoroutine(StartDialogue());
    }

    private IEnumerator StartDialogue()
    {
        if (delayBeforeDialogue > 0f)
        {
            yield return new WaitForSeconds(delayBeforeDialogue);
        }

        StartCoroutine(dialogueUI.PlayLines(lines));
    }
}