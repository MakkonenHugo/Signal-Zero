using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class DialogueTrigger : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public List<DialogueLine> lines = new List<DialogueLine>();

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

        hasTriggered = true;

        StartCoroutine(dialogueUI.PlayLines(lines));
    }
}