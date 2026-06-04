using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public AudioClip gameMusic;

    void Start()
    {
        AudioManager.Instance?.PlayMusic(gameMusic);
    }
}