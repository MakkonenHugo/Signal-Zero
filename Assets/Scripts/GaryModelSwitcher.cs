using UnityEngine;

public class GaryModelSwitcher : MonoBehaviour
{
    public GameObject unarmedModel;
    public GameObject armedModel;

    public void SwitchToArmed()
    {
        if (unarmedModel != null)
            unarmedModel.SetActive(false);

        if (armedModel != null)
            armedModel.SetActive(true);
    }
}