using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BaseStationExit : MonoBehaviour
{
    [Header("Scene transition")]
    public string outsideSceneName = "SampleScene";
    public string spawnPointId = "base_exit";

    [Header("Timing")]
    public float walkOutDuration = 0.6f;

    [Header("Audio")]
    public AudioClip doorOpenSFX;

    [Header("References")]
    public Transform exitPoint;

    private bool playerInRange = false;
    private bool isTransitioning = false;
    private Transform currentPlayer;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("BaseStationExit OnTriggerEnter2D fired");
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        currentPlayer = other.transform;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        currentPlayer = null;
    }

    void Update()
    {
        if (!playerInRange || isTransitioning) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(ExitSequence(currentPlayer));
        }
    }

    IEnumerator ExitSequence(Transform player)
    {
        isTransitioning = true;

        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null) playerController.enabled = false;

        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null) playerHealth.isIndoors = false;

        AudioManager.Instance?.PlaySFX(doorOpenSFX);

        if (exitPoint != null)
        {
            Vector3 start = player.position;
            Vector3 target = exitPoint.position;
            float elapsed = 0f;

            var anim = player.GetComponent<Animator>();
            if (anim != null) { anim.enabled = true; anim.Play("walking-front"); }

            while (elapsed < walkOutDuration)
            {
                elapsed += Time.deltaTime;
                player.position = Vector3.Lerp(start, target, elapsed / walkOutDuration);
                yield return null;
            }
        }

        if (SceneFader.Instance != null)
            yield return StartCoroutine(SceneFader.Instance.FadeOut());

        PlayerPrefs.SetString("SpawnPoint", spawnPointId);
        PlayerPrefs.SetString("DoorState", "closed");
        UnityEngine.SceneManagement.SceneManager.LoadScene(outsideSceneName);
    }
}