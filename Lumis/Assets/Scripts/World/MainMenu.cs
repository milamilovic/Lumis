using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public Image fadeOverlay;

    [Header("Settings sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider radiationSlider;

    void Start()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f; // in case player came from paused game

        UpdateSliderValues();

        StartCoroutine(InitSliders());

        AudioManager.Instance?.PlayMusic(AudioManager.Instance.mainMenuMusic);

        var hotbar = GameObject.Find("HotbarPanel");
        if (hotbar != null) hotbar.SetActive(false);
        var health = GameObject.Find("HealthBar");
        if (health != null) health.SetActive(false);

        StartCoroutine(FadeInScene());
    }

    IEnumerator FadeInScene()
    {
        yield return new WaitForSeconds(0.1f);
        if (SceneFader.Instance != null)
            yield return StartCoroutine(SceneFader.Instance.FadeIn());
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

    public void StartGame()
    {
        StartCoroutine(FadeAndLoad("SampleScene"));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        AudioManager.Instance?.FadeOutMusic();
        float duration = AudioManager.Instance != null
            ? AudioManager.Instance.fadeOutDuration
            : 1.5f;
        float timer = 0f;
        Color c = fadeOverlay.color;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, timer / duration);
            fadeOverlay.color = c;
            yield return null;
        }
        c.a = 1f;
        fadeOverlay.color = c;
        yield return new WaitForSecondsRealtime(0.1f);

        SaveManager.Instance?.ClearSceneSnapshot();
        SaveManager.Instance?.ResetSession();
        CollectedPickupsTracker.Instance?.Clear();

        var hotbar = GameObject.Find("HotbarPanel");
        if (hotbar != null) hotbar.SetActive(true);
        var health = GameObject.Find("HealthBar");
        if (health != null) health.SetActive(true);

        SceneManager.LoadScene(sceneName);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
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