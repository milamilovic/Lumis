using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public AudioClip gameMusic;

    void Awake()
    {
        SaveManager.Instance?.RestoreIfPending();
    }

    void Start()
    {
        AudioManager.Instance?.PlayMusic(gameMusic);
    }
}