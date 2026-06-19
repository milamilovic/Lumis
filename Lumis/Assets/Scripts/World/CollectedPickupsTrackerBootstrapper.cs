using UnityEngine;

public class CollectedPickupsTrackerBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create()
    {
        if (Object.FindAnyObjectByType<CollectedPickupsTracker>() != null) return;

        var obj = new GameObject("CollectedPickupsTracker");
        obj.AddComponent<CollectedPickupsTracker>();
        Object.DontDestroyOnLoad(obj);
        Debug.Log("CollectedPickupsTracker auto-created");
    }
}