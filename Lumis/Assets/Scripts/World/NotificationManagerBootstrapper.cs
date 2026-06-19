using UnityEngine;

public class NotificationManagerBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create()
    {
        if (Object.FindAnyObjectByType<NotificationManager>() != null) return;

        var prefab = Resources.Load<GameObject>("NotificationManagerPrefab");
        if (prefab == null)
        {
            Debug.LogError("NotificationManagerPrefab not found in Resources!");
            return;
        }

        var obj = Object.Instantiate(prefab);
        obj.name = "NotificationManager";
        Object.DontDestroyOnLoad(obj);
    }
}