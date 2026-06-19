using UnityEngine;

public class AudioManagerBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void CreateAudioManager()
    {
        if (Object.FindAnyObjectByType<AudioManager>() != null) return;

        var prefab = Resources.Load<GameObject>("AudioManager");
        if (prefab == null)
        {
            Debug.LogError("AudioManager not found in Resources folder!");
            return;
        }

        var obj = Object.Instantiate(prefab);
        obj.name = "AudioManager";
        Object.DontDestroyOnLoad(obj);
        Debug.Log("AudioManager auto-created");
    }
}