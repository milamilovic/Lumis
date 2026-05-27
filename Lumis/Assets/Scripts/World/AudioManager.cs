using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip backgroundMusic;

    [Header("Radiation Geiger")]
    public AudioSource[] geigerSources;
    public AudioClip geigerLow;
    public AudioClip geigerMedium;
    public AudioClip geigerHigh;

    float musicVolume = 1f;
    float sfxVolume = 1f;
    float radiationVolume = 0.5f;

    private float lastRadiation = 0f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LoadVolumeSettings();
        PlayMusic(backgroundMusic);

        AudioClip[] clips = { geigerLow, geigerMedium, geigerHigh };
        for (int i = 0; i < geigerSources.Length; i++)
        {
            if (clips[i] == null || geigerSources[i] == null) continue;
            geigerSources[i].clip = clips[i];
            geigerSources[i].loop = true;
            geigerSources[i].volume = 0f;
            geigerSources[i].Play();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
        ApplyVolumes();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetRadiationVolume(float radiation)
    {
        lastRadiation = radiation;

        float[] targets = {
            Mathf.InverseLerp(0.5f, 0f,  radiation),      // low:    loud at 0, silent at 0.5
            1f - Mathf.Abs(radiation - 0.5f) / 0.5f,      // medium: peaks at 0.5
            Mathf.InverseLerp(0.5f, 1f,  radiation)       // high:   silent at 0.5, loud at 1
        };

        float masterSFX = sfxVolume;

        for (int i = 0; i < geigerSources.Length; i++)
        {
            if (geigerSources[i] == null) continue;
            float target = Mathf.Clamp01(targets[i]) * radiationVolume * 0.3f;
            geigerSources[i].volume = target;
        }
    }

    public void SetMusicVolume(float v) { musicVolume = v; ApplyVolumes(); SaveVolumeSettings(); }
    public void SetSFXVolume(float v) { sfxVolume = v; ApplyVolumes(); SaveVolumeSettings(); }
    public void SetRadiationVolume_Slider(float v)
    {
        radiationVolume = v;
        SetRadiationVolume(lastRadiation);
        SaveVolumeSettings();
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
    public float GetRadiationVolume() => radiationVolume;

    void ApplyVolumes()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("RadiationVolume", radiationVolume);
        PlayerPrefs.Save();
    }

    void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        radiationVolume = PlayerPrefs.GetFloat("RadiationVolume", 0.5f);
        ApplyVolumes();
    }
}