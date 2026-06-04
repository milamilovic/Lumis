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
        StartCoroutine(NewGameCoroutine());
    }

    IEnumerator NewGameCoroutine()
    {
        losePanel.SetActive(false);
        Time.timeScale = 1f;

        yield return null;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}