using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;
    public GameObject fadeOverlay;
    public float fadeDuration = 1.5f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        Image image = fadeOverlay.GetComponentInChildren<Image>();
        Color c = image.color;
        c.a = 1f;
        image.color = c;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            image.color = c;
            yield return null;
        }

        c.a = 0f;
        image.color = c;
    }

    public IEnumerator FadeOut()
    {
        Image image = fadeOverlay.GetComponentInChildren<Image>();
        Color c = image.color;
        c.a = 0f;
        image.color = c;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            image.color = c;
            yield return null;
        }

        c.a = 1f;
        image.color = c;
    }
}