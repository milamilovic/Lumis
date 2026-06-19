using System.Collections;
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

        var playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null) playerController.enabled = true;

        StartCoroutine(DelayedMusicStart());
        StartCoroutine(FadeInScene());
    }

    IEnumerator FadeInScene()
    {
        yield return new WaitForSeconds(0.1f);
        if (SceneFader.Instance != null)
        {
            yield return StartCoroutine(SceneFader.Instance.FadeIn());
        }
        else
        {
            Debug.LogWarning("SceneFader.Instance is null!");
        }
    }

    IEnumerator DelayedMusicStart()
    {
        yield return null;
        AudioManager.Instance?.PlayMusic(gameMusic);
    }
}