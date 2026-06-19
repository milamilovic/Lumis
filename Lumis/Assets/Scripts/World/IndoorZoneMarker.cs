using UnityEngine;
using System.Collections;

public class IndoorZoneMarker : MonoBehaviour
{

    public AudioClip indoorMusic;
    void Start()
    {
        Debug.Log("IndoorZoneMarker Start - entering interior scene");

        var player = FindFirstObjectByType<PlayerHealth>();
        if (player != null) player.isIndoors = true;

        var playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        AudioManager.Instance?.PlayMusic(indoorMusic);
        AudioManager.Instance?.SetMusicVolume((float)(AudioManager.Instance?.GetMusicVolume() * 2));

        AudioManager.Instance?.SetFootstepsVolume((float)(AudioManager.Instance?.GetFootstepsVolume() * 2));

        Debug.Log($"SceneFader.Instance before FadeInScene: {SceneFader.Instance}");
        StartCoroutine(FadeInScene());
    }

    IEnumerator FadeInScene()
    {
        Debug.Log("FadeInScene coroutine started");
        yield return new WaitForSeconds(0.1f);
        if (SceneFader.Instance != null)
        {
            Debug.Log("Calling FadeIn");
            yield return StartCoroutine(SceneFader.Instance.FadeIn());
            Debug.Log("FadeIn finished");
        }
        else
        {
            Debug.LogWarning("SceneFader.Instance null in interior scene!");
        }
    }

    void OnDestroy()
    {
        var player = FindFirstObjectByType<PlayerHealth>();
        if (player != null) player.isIndoors = false;
        var playerController = FindFirstObjectByType<PlayerController>();
        AudioManager.Instance?.SetMusicVolume((float)(AudioManager.Instance?.GetMusicVolume() / 2));
    }
}