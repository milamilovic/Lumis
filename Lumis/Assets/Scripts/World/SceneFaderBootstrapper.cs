using UnityEngine;

public class SceneFaderBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void CreateSceneFader()
    {
        Debug.Log("SceneFaderBootstrapper running");

        if (Object.FindAnyObjectByType<SceneFader>() != null)
        {
            Debug.Log("SceneFader already exists, skipping creation");
            return;
        }

        var prefab = Resources.Load<GameObject>("SceneFader");
        if (prefab == null)
        {
            Debug.LogError("SceneFader not found in Resources!");
            return;
        }

        var obj = Object.Instantiate(prefab);
        obj.name = "SceneFader";
        Object.DontDestroyOnLoad(obj);
        Debug.Log("SceneFader auto-created successfully");
    }
}