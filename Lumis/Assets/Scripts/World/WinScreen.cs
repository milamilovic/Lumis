using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public static WinScreen Instance;
    public GameObject winPanel;

    private bool robotCrafted = false;

    void Awake()
    {
        Instance = this;
        winPanel.SetActive(false);
    }

    public void NotifyRobotCrafted()
    {
        robotCrafted = true;
    }

    void Update()
    {
        if (robotCrafted && Keyboard.current.qKey.wasPressedThisFrame)
            Show();
    }

    public void Show()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void NewGame()
    {
        StartCoroutine(NewGameCoroutine());
    }

    IEnumerator NewGameCoroutine()
    {
        winPanel.SetActive(false);
        Time.timeScale = 1f;
        AudioManager.Instance?.FadeOutMusic();

        if (SceneFader.Instance != null)
            yield return StartCoroutine(SceneFader.Instance.FadeOut());
        else
            yield return new WaitForSecondsRealtime(1.5f);

        SaveManager.Instance?.ClearSceneSnapshot();
        SaveManager.Instance?.ResetSession();
        CollectedPickupsTracker.Instance?.Clear();

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