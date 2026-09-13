using UnityEngine;

public class DeactivatePromptManager : MonoBehaviour
{
    public Transform player;
    public GameObject promptUI;
    public float searchRadius = 3f;

    private void Update()
    {
        EnemyDeactivator nearest = FindNearestDeactivatable();

        if (promptUI != null)
        {
            promptUI.SetActive(nearest != null);
        }
    }

    private EnemyDeactivator FindNearestDeactivatable()
    {
        if (player == null)
            return null;

        EnemyDeactivator[] deactivators = FindObjectsByType<EnemyDeactivator>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        EnemyDeactivator nearest = null;
        float nearestDistance = searchRadius;

        foreach (EnemyDeactivator deactivator in deactivators)
        {
            if (!deactivator.enabled || !deactivator.IsInRange || !deactivator.CanBeDeactivated)
                continue;

            float distance = Vector3.Distance(player.position, deactivator.transform.position);

            if (distance <= nearestDistance)
            {
                nearest = deactivator;
                nearestDistance = distance;
            }
        }

        return nearest;
    }
}