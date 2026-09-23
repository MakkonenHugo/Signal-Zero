using UnityEngine;

public class LoadoutSelector : MonoBehaviour
{
    public GameObject loadoutPanel;
    public GameObject pistol;
    public GameObject smg;

    public GameObject rifle;

    private void Start()
    {
        Time.timeScale = 0f;

        if (loadoutPanel != null)
        {
            loadoutPanel.SetActive(true);
        }

        if (pistol != null)
            pistol.SetActive(false);

        if (smg != null)
            smg.SetActive(false);

        if (rifle != null)
            rifle.SetActive(false);
    }

    public void SelectPistol()
    {
        if (pistol != null)
            pistol.SetActive(true);

        if (smg != null)
            smg.SetActive(false);

        if (rifle != null)
            rifle.SetActive(false);

        ConfirmSelection();
    }

    public void SelectSmg()
    {
        if (smg != null)
            smg.SetActive(true);

            pistol.SetActive(false);

        if (rifle != null)
            rifle.SetActive(false);

        ConfirmSelection();
    }

    public void SelectRifle()
    {
        if (rifle != null)
            rifle.SetActive(true);

        if (smg != null)
            smg.SetActive(false);

        if (pistol != null)
            pistol.SetActive(false);

        ConfirmSelection();
    }

    private void ConfirmSelection()
    {
        if (loadoutPanel != null)
        {
            loadoutPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}