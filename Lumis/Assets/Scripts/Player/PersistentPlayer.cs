using UnityEngine;

public class PersistentPlayer : MonoBehaviour
{
    private static PersistentPlayer Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
