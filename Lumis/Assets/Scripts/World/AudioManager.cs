using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip backgroundMusic;
    public AudioClip pickupSFX;
    public AudioClip mainMenuMusic;

    [Header("Radiation Geiger")]
    public AudioSource geigerLow;
    public AudioSource geigerMedium;
    public AudioSource geigerHigh;
    public AudioClip geigerLowClip;
    public AudioClip geigerMediumClip;
    public AudioClip geigerHighClip;

    [Header("Radiation smoothing")]
    public float radiationFadeSpeed = 2f;

    [Header("Music fade")]
    public float fadeOutDuration = 1.5f;
    public float fadeInDuration = 1.5f;

    float masterVolume = 1f;
    float musicVolume = 1f;
    float sfxVolume = 1f;
    float radiationVolume = 0.5f;
    private float lastRadiation = 0f;
    private bool isFading = false;
    private Coroutine fadeMusicCoroutine;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadVolumeSettings();
    }

    void Start()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentScene == "MainMenu")
            PlayMusic(mainMenuMusic);
        else
            PlayMusic(backgroundMusic);
        InitGeigerSource(geigerLow, geigerLowClip);
        InitGeigerSource(geigerMedium, geigerMediumClip);
        InitGeigerSource(geigerHigh, geigerHighClip);
    }

    void InitGeigerSource(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) return;
        source.clip = clip;
        source.loop = true;
        source.volume = 0f;
        source.Play();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (fadeMusicCoroutine != null)
            StopCoroutine(fadeMusicCoroutine);
        fadeMusicCoroutine = StartCoroutine(FadeMusic(clip));
    }

    IEnumerator FadeMusic(AudioClip newClip)
    {
        isFading = true;

        if (musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            float timer = 0f;

            while (timer < fadeOutDuration)
            {
                timer += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeOutDuration);
                yield return null;
            }

            musicSource.Stop();
            musicSource.volume = 0f;
        }

        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();
        musicSource.time = (newClip == backgroundMusic) ? 2f : 0f;

        float targetVolume = musicVolume * masterVolume;
        float fadeTimer = 0f;

        while (fadeTimer < fadeInDuration)
        {
            fadeTimer += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, fadeTimer / fadeInDuration);
            yield return null;
        }

        musicSource.volume = targetVolume;
        isFading = false;
    }

    public void FadeOutMusic()
    {
        if (fadeMusicCoroutine != null)
            StopCoroutine(fadeMusicCoroutine);
        fadeMusicCoroutine = StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        isFading = true;
        float startVolume = musicSource.volume;
        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeOutDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = 0f;
        isFading = false;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    public void PlayPickup() => PlaySFX(pickupSFX);

    public void SetRadiationVolume(float radiation)
    {
        lastRadiation = radiation;

        float targetLow = 0f;
        float targetMedium = 0f;
        float targetHigh = 0f;

        if (radiation < 0.2f) { }
        else if (radiation < 0.3f)
        {
            // gradual low buildup
            targetLow = Mathf.InverseLerp(0.2f, 0.3f, radiation);
        }
        else if (radiation < 0.4f)
        {
            // full low
            targetLow = 1f;
        }
        else if (radiation < 0.5f)
        {
            // low fading out, medium fading in
            float t = Mathf.InverseLerp(0.4f, 0.5f, radiation);
            targetLow = 1f - t;
            targetMedium = t;
        }
        else if (radiation < 0.7f)
        {
            // full medium
            targetMedium = 1f;
        }
        else if (radiation < 0.8f)
        {
            // medium fading out, high fading in
            float t = Mathf.InverseLerp(0.7f, 0.8f, radiation);
            targetMedium = 1f - t;
            targetHigh = t;
        }
        else
        {
            // full high
            targetHigh = 1f;
        }

        float scale = radiationVolume * masterVolume;
        FadeGeigerTo(geigerLow, targetLow * scale);
        FadeGeigerTo(geigerMedium, targetMedium * scale);
        FadeGeigerTo(geigerHigh, targetHigh * scale);
    }

    void FadeGeigerTo(AudioSource source, float target)
    {
        if (source == null) return;
        source.volume = Mathf.MoveTowards(
            source.volume, target,
            radiationFadeSpeed * Time.deltaTime);
    }

    public void SetMasterVolume(float v)
    {
        masterVolume = v;
        ApplyVolumes();
        SetRadiationVolume(lastRadiation);
        SaveVolumeSettings();
    }

    public void SetMusicVolume(float v) { musicVolume = v; ApplyVolumes(); SaveVolumeSettings(); }
    public void SetSFXVolume(float v) { sfxVolume = v; ApplyVolumes(); SaveVolumeSettings(); }
    public void SetRadiationVolume_Slider(float v)
    {
        radiationVolume = v;
        SetRadiationVolume(lastRadiation);
        SaveVolumeSettings();
    }

    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
    public float GetRadiationVolume() => radiationVolume;

    void ApplyVolumes()
    {
        if (!isFading)
            musicSource.volume = musicVolume * masterVolume;
        sfxSource.volume = sfxVolume * masterVolume;
    }

    void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("RadiationVolume", radiationVolume);
        PlayerPrefs.Save();
    }

    void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        radiationVolume = PlayerPrefs.GetFloat("RadiationVolume", 0.5f);
        ApplyVolumes();
    }
}