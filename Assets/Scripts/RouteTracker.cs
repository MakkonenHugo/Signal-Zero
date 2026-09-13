using UnityEngine;

public enum RouteResult
{
    Terminator,
    Android,
    Neutral
}

public class RouteTracker : MonoBehaviour
{
    public static RouteTracker Instance { get; private set; }

    private int killCount;
    private int deactivateCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterKill()
    {
        killCount++;
    }

    public void RegisterDeactivation()
    {
        deactivateCount++;
    }

    public RouteResult GetResult()
    {
        if (deactivateCount > 0 && killCount == 0)
        {
            return RouteResult.Android;
        }

        if (killCount > 0 && deactivateCount == 0)
        {
            return RouteResult.Terminator;
        }

        return RouteResult.Neutral;
    }
}