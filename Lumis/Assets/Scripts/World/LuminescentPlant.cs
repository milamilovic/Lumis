using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LuminescentPlant : MonoBehaviour
{
    public float minBrightness = 0.8f;
    public float maxBrightness = 1.6f;
    public float pulseSpeed = 1.3f;

    private Light2D light2D;
    private SpriteRenderer sr;
    private Color baseColor;
    private float time;

    void Start()
    {
        light2D = GetComponentInChildren<Light2D>();
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
    }

    void Update()
    {
        time += Time.deltaTime * pulseSpeed;
        float brightness = Mathf.Lerp(minBrightness, maxBrightness, (Mathf.Sin(time) + 1f) / 2f);

        if (light2D != null)
            light2D.intensity = brightness;

        // changes just the color not alpha
        Color newColor = baseColor * brightness;
        newColor.a = sr.color.a;
        sr.color = newColor;
    }
}