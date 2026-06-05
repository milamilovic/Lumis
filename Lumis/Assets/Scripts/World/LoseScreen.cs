using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoseScreen : MonoBehaviour
{
    public static LoseScreen Instance;
    public GameObject losePanel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        losePanel.SetActive(false);
    }

    public void Show()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.Instance?.SetRadiationVolume(0f);
    }

    public void NewGame()
    {
        StartCoroutine(RestartCoroutine());
    }

    IEnumerator RestartCoroutine()
    {
        losePanel.SetActive(false);
        AudioManager.Instance?.FadeOutMusic();
        if (SceneFader.Instance != null)
            yield return StartCoroutine(SceneFader.Instance.FadeOut());
        else
            yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        StartCoroutine(FadeAndLoad("MainMenu"));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        Time.timeScale = 1f;
        AudioManager.Instance?.FadeOutMusic();
        if (SceneFader.Instance != null)
            yield return StartCoroutine(SceneFader.Instance.FadeOut());
        else
            yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadScene(sceneName);
    }
}