using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;
    public Image fadeOverlay;
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
        Color c = fadeOverlay.color;
        c.a = 1f;
        fadeOverlay.color = c;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeOverlay.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeOverlay.color = c;
    }

    public IEnumerator FadeOut()
    {
        Color c = fadeOverlay.color;
        c.a = 0f;
        fadeOverlay.color = c;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeOverlay.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeOverlay.color = c;
    }
}