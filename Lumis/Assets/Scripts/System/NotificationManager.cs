using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    public RectTransform banner;
    public TextMeshProUGUI titleLabel;
    public TextMeshProUGUI bodyLabel;
    public AudioClip checkpointSFX;

    public float visibleX = -200f;     // Top-right resting position
    public float hiddenX = 150f;      // Off-screen right position

    public float slideDuration = 0.4f;
    public float displayDuration = 3f;

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

    private void Start()
    {
        banner.anchoredPosition = new Vector2(hiddenX, banner.anchoredPosition.y);
        banner.gameObject.SetActive(false);
    }

    public void ShowNotification(string title, string body)
    {
        Debug.Log($"ShowNotification called: {title}");
        StopAllCoroutines();
        StartCoroutine(AnimateBanner(title, body));
    }

    IEnumerator AnimateBanner(string title, string body)
    {
        titleLabel.text = title;
        bodyLabel.text = body;

        banner.gameObject.SetActive(true);

        AudioManager.Instance?.PlaySFX(checkpointSFX);

        // Slide in from right
        yield return StartCoroutine(SlideToX(hiddenX, visibleX, slideDuration));

        // Hold
        yield return new WaitForSecondsRealtime(displayDuration);

        // Slide out to right
        yield return StartCoroutine(SlideToX(visibleX, hiddenX, slideDuration));

        banner.gameObject.SetActive(false);
    }

    IEnumerator SlideToX(float fromX, float toX, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            banner.anchoredPosition = new Vector2(
                Mathf.Lerp(fromX, toX, t),
                banner.anchoredPosition.y
            );

            yield return null;
        }

        banner.anchoredPosition = new Vector2(toX, banner.anchoredPosition.y);
    }
}