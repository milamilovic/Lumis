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

        yield return null;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}