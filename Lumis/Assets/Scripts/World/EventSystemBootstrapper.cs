using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create()
    {
        if (Object.FindAnyObjectByType<EventSystem>() != null) return;

        var prefab = Resources.Load<GameObject>("EventSystem");
        if (prefab == null)
        {
            Debug.LogError("EventSystem not found in Resources!");
            return;
        }

        var obj = Object.Instantiate(prefab);
        obj.name = "EventSystem";
        Object.DontDestroyOnLoad(obj);
    }
}