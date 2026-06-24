using System.Collections;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public AudioClip gameMusic;

    void Awake()
    {
        Debug.Log("=== GameBootstrap AWAKE ===");
        SaveManager.Instance?.RestoreCollectedPickupsEarly();
        SaveManager.Instance?.RestoreIfPending();
    }

    void Start()
    {
        var playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null) playerController.enabled = true;

        StartCoroutine(DelayedMusicStart());
        StartCoroutine(FadeInScene());

        var hotbar = FindIncludingInactive("HotbarPanel");
        if (hotbar != null) hotbar.SetActive(true);
        var health = FindIncludingInactive("HealthBar");
        if (health != null) health.SetActive(true);
        var player = FindIncludingInactive("Player");
        if (player != null) player.SetActive(true);

        bool wasReturningFromSnapshot = SaveManager.Instance != null
            && SaveManager.Instance.HasActiveSnapshotOrRestore();

        SaveManager.Instance?.RestoreSceneSnapshot();

        if (!wasReturningFromSnapshot)
        {
            var inventory = FindFirstObjectByType<Inventory>();
            inventory?.ClearAll();
            var playerHealth = FindFirstObjectByType<PlayerHealth>();
            playerHealth?.Heal();
            if (playerController != null)
            {
                playerController.transform.position = Vector3.zero;
                playerController.FaceForward();
            }
        }
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

    GameObject FindIncludingInactive(string name)
    {
        var allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (var t in allTransforms)
            if (t.name == name && t.gameObject.scene.isLoaded)
                return t.gameObject;
        return null;
    }
}