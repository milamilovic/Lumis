using UnityEngine;
using System.Collections;

public class IndoorZoneMarker : MonoBehaviour
{

    public AudioClip indoorMusic;
    void Start()
    {
        var player = FindFirstObjectByType<PlayerHealth>();
        if (player != null) player.isIndoors = true;

        var playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null) playerController.enabled = true;

        AudioManager.Instance?.PlayMusic(indoorMusic);
        AudioManager.Instance?.SetMusicVolume((float)(AudioManager.Instance?.GetMusicVolume() * 2));

        StartCoroutine(FadeInScene());
    }

    IEnumerator FadeInScene()
    {
        yield return new WaitForSeconds(0.1f);
        if (SceneFader.Instance != null)
            yield return StartCoroutine(SceneFader.Instance.FadeIn());
    }

    void OnDestroy()
    {
        var player = FindFirstObjectByType<PlayerHealth>();
        if (player != null) player.isIndoors = false;
        AudioManager.Instance?.SetMusicVolume((float)(AudioManager.Instance?.GetMusicVolume() / 2));
    }
}