using UnityEngine;
using UnityEngine.InputSystem;

public class OverrideController : MonoBehaviour
{
    public float timeScale = 0.3f;
    public float maxMeter = 5f;
    public float drainRate = 1f;
    public float rechargeRate = 0.5f;
    public float rechargeDelay = 1f;

    private float currentMeter;
    private bool isActive;
    private float rechargeDelayTimer;

    public float MeterFraction => currentMeter / maxMeter;
    public bool IsActive => isActive;

    private void Awake()
    {
        currentMeter = maxMeter;
    }

    private void Update()
    {
        bool wantsOverride = Mouse.current != null && Mouse.current.rightButton.isPressed;

        if (wantsOverride && currentMeter > 0f)
        {
            isActive = true;
        }
        else
        {
            isActive = false;
        }

        if (isActive)
        {
            currentMeter -= drainRate * Time.unscaledDeltaTime;
            currentMeter = Mathf.Max(currentMeter, 0f);
            rechargeDelayTimer = rechargeDelay;
        }
        else
        {
            if (rechargeDelayTimer > 0f)
            {
                rechargeDelayTimer -= Time.unscaledDeltaTime;
            }
            else if (currentMeter < maxMeter)
            {
                currentMeter += rechargeRate * Time.unscaledDeltaTime;
                currentMeter = Mathf.Min(currentMeter, maxMeter);
            }
        }

        Time.timeScale = isActive ? timeScale : 1f;
    }
}