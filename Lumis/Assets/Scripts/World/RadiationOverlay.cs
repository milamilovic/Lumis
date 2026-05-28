using UnityEngine;
using UnityEngine.UI;

public class RadiationOverlay : MonoBehaviour
{
    public RawImage staticOverlay;
    public float updateInterval = 0.05f;

    private Texture2D noiseTexture;
    private float timer;
    private float currentRadiation;

    private const int TEX_SIZE = 512;

    void Start()
    {
        // noise texture
        noiseTexture = new Texture2D(TEX_SIZE, TEX_SIZE, TextureFormat.RGBA32, false);
        noiseTexture.filterMode = FilterMode.Point;
        noiseTexture.wrapMode = TextureWrapMode.Repeat;
        staticOverlay.texture = noiseTexture;
    }

    void Update()
    {
        if (RadiationManager.Instance != null)
        {
            var player = FindFirstObjectByType<PlayerHealth>();
            if (player != null)
                currentRadiation = RadiationManager.Instance.GetRadiationAt(player.transform.position);
        }

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            if (currentRadiation > 0.2f)
                RefreshNoise();
        }

        // fade overlay based on radiation
        float targetAlpha = Mathf.InverseLerp(0.4f, 1f, currentRadiation) * 0.30f;
        var c = staticOverlay.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 3f);
        staticOverlay.color = c;
    }

    void RefreshNoise()
    {
        Color[] pixels = noiseTexture.GetPixels();
        float intensity = Mathf.InverseLerp(0.2f, 1f, currentRadiation);

        for (int i = 0; i < pixels.Length; i++)
        {

            if (Random.value < intensity * 0.12f)
            {
                float v = Random.value < intensity ? Random.value : 0f;
                pixels[i] = new Color(v, v, v, Random.Range(0.3f, 0.7f));
            }
            else
            {
                pixels[i] = Color.clear;
            }
        }

        noiseTexture.SetPixels(pixels);
        noiseTexture.Apply();
    }
}