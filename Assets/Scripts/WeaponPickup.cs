using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour
{
    public string playerTag = "Player";

    public GameObject weaponToEnable;
    public MonoBehaviour weaponScriptToEnable;
    public GameObject promptObject;
    public GameObject pickupVisual;

    public AudioSource audioSource;
    public AudioClip pickupSound;

    public bool requireKeyPress = true;

    private bool playerInRange;
    private bool collected;

    private void Start()
    {
        if (weaponToEnable != null)
            weaponToEnable.SetActive(false);

        if (weaponScriptToEnable != null)
            weaponScriptToEnable.enabled = false;

        if (promptObject != null)
            promptObject.SetActive(false);
    }

    private void Update()
    {
        if (collected || !playerInRange || !requireKeyPress)
            return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Collect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        if (!other.transform.root.CompareTag(playerTag))
            return;

        playerInRange = true;

        if (requireKeyPress)
        {
            if (promptObject != null)
                promptObject.SetActive(true);
        }
        else
        {
            Collect();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.CompareTag(playerTag))
            return;

        playerInRange = false;

        if (promptObject != null)
            promptObject.SetActive(false);
    }

    private void Collect()
    {
        collected = true;

        if (weaponToEnable != null)
            weaponToEnable.SetActive(true);

        if (weaponScriptToEnable != null)
            weaponScriptToEnable.enabled = true;

        if (promptObject != null)
            promptObject.SetActive(false);

        if (audioSource != null && pickupSound != null)
            audioSource.PlayOneShot(pickupSound);

        if (pickupVisual != null)
            pickupVisual.SetActive(false);
    }
}