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

        UpdateSliderValues();

        StartCoroutine(InitSliders());
    }

    IEnumerator InitSliders()
    {
        yield return null;
        UpdateSliderValues();
        masterSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetMasterVolume(v));
        musicSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetMusicVolume(v));
        sfxSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetSFXVolume(v));
        radiationSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetRadiationVolume_Slider(v));
        AudioManager.Instance?.PlayMusic(AudioManager.Instance.mainMenuMusic);
    }

    void UpdateSliderValues()
    {
        if (AudioManager.Instance == null) return;

        masterSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        radiationSlider.onValueChanged.RemoveAllListeners();

        masterSlider.value = AudioManager.Instance.GetMasterVolume();
        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();
        radiationSlider.value = AudioManager.Instance.GetRadiationVolume();
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
        AudioManager.Instance?.FadeOutMusic();
        if (SceneFader.Instance != null)
            yield return StartCoroutine(SceneFader.Instance.FadeOut());
        else
            yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
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