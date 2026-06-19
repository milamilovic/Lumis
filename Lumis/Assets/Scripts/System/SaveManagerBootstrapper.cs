using UnityEngine;

public class SaveManagerBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void CreateSaveManager()
    {
        var obj = new GameObject("SaveManager");
        obj.AddComponent<SaveManager>();
        Object.DontDestroyOnLoad(obj);
        Debug.Log("SaveManager auto-created");
    }
}