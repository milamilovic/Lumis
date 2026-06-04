using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider radiationSlider;

    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);

        if (AudioManager.Instance != null)
        {
            masterSlider.value = AudioManager.Instance.GetMasterVolume();
            musicSlider.value = AudioManager.Instance.GetMusicVolume();
            sfxSlider.value = AudioManager.Instance.GetSFXVolume();
            radiationSlider.value = AudioManager.Instance.GetRadiationVolume();
        }

        masterSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetMasterVolume(v));
        musicSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetMusicVolume(v));
        sfxSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetSFXVolume(v));
        radiationSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetRadiationVolume_Slider(v));
    }

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame ||
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        StartCoroutine(RestartCoroutine());
    }

    IEnumerator RestartCoroutine()
    {
        Time.timeScale = 1f;
        yield return null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}