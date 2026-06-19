using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BaseStationDoor : MonoBehaviour
{
    [Header("Scene transition")]
    public string interiorSceneName = "BaseStationInterior";
    public string spawnPointId = "base_entrance";

    [Header("Timing")]
    public float openingDuration = 0.4f;
    public float pauseAfterOpen = 0.4f;
    public float closingDuration = 0.4f;
    public float shrinkDuration = 0.4f;

    [Header("Audio")]
    public AudioClip doorOpenSFX;
    public AudioClip doorCloseSFX;

    [Header("References")]
    public Transform doorCenterPoint;

    private Animator anim;
    private bool playerInRange = false;
    private bool isTransitioning = false;
    private Transform currentPlayer;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("BaseStationDoor OnTriggerEnter2D fired");
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
            StartCoroutine(EnterSequence(currentPlayer));
        }
    }

    IEnumerator EnterSequence(Transform player)
    {
        isTransitioning = true;

        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null) playerController.enabled = false;

        anim.Play("base-station-opening");
        AudioManager.Instance?.PlaySFX(doorOpenSFX);

        yield return new WaitForSeconds(openingDuration);
        anim.Play("base-station-open");

        yield return new WaitForSeconds(pauseAfterOpen);

        anim.Play("base-station-closing");
        AudioManager.Instance?.PlaySFX(doorCloseSFX);

        if (SceneFader.Instance != null)
            StartCoroutine(SceneFader.Instance.FadeOut());

        var playerAnim = player.GetComponent<Animator>();
        var playerSR = player.GetComponent<SpriteRenderer>();
        if (playerAnim != null)
        {
            playerAnim.enabled = true;
            playerAnim.Play("walking-back");
        }
        if (playerSR != null)
            playerSR.flipX = false;

        Vector3 startPos = player.position;
        Vector3 startScale = player.localScale;
        Vector3 targetPos = doorCenterPoint != null ? doorCenterPoint.position : transform.position;
        Vector3 targetScale = startScale * 0.2f;

        float elapsed = 0f;
        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkDuration;
            player.position = Vector3.Lerp(startPos, targetPos, t);
            player.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        float remaining = closingDuration - shrinkDuration;
        if (remaining > 0f)
            yield return new WaitForSeconds(remaining);

        PlayerPrefs.SetString("SpawnPoint", spawnPointId);
        SaveManager.Instance?.CaptureSceneSnapshot();
        UnityEngine.SceneManagement.SceneManager.LoadScene(interiorSceneName);
    }
}