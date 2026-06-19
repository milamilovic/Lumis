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
        Debug.Log("GameBootstrap.Start() running");
        Debug.Log($"AudioManager.Instance: {AudioManager.Instance}");
        Debug.Log($"gameMusic assigned: {gameMusic != null}");
        StartCoroutine(DelayedMusicStart());
        StartCoroutine(FadeInScene());
    }

    IEnumerator FadeInScene()
    {
        Debug.Log("FadeInScene starting in outdoor scene");
        yield return new WaitForSeconds(0.1f);
        if (SceneFader.Instance != null)
        {
            Debug.Log("Calling SceneFader.FadeIn()");
            yield return StartCoroutine(SceneFader.Instance.FadeIn());
            Debug.Log("FadeIn complete");
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