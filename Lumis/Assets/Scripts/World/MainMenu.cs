using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel;

    [Header("Settings sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider radiationSlider;

    void Start()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f; // in case player came from paused game

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
        radiationSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetRadiationVolume(v));

        AudioManager.Instance?.PlayMusic(AudioManager.Instance.mainMenuMusic);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SamepleScene");
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
        Application.Quit();
    }
}