using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [Header("Durations (seconds)")]
    public float dayDuration = 60f;
    public float nightDuration = 60f;
    public float transitionDuration = 12.5f;

    [Header("Colors")]
    public Color dayColor = new Color(0.7f, 0.7f, 0.75f, 1f);
    public Color nightColor = new Color(0.1f, 0.1f, 0.15f, 1f);

    private Light2D globalLight;
    private float currentTime = 0f;
    private float cycleDuration;

    void Start()
    {
        globalLight = GetComponent<Light2D>();
        cycleDuration = dayDuration + nightDuration + (transitionDuration * 2f);
        globalLight.color = dayColor;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= cycleDuration) currentTime = 0f;

        globalLight.color = GetCurrentColor();
    }

    Color GetCurrentColor()
    {
        float t = currentTime;
        float dayEnd = dayDuration;
        float dawnEnd = dayEnd + transitionDuration;
        float nightEnd = dawnEnd + nightDuration;

        if (t < dayEnd)
            return dayColor;

        if (t < dawnEnd)
            return Color.Lerp(dayColor, nightColor, (t - dayEnd) / transitionDuration);

        if (t < nightEnd)
            return nightColor;

        return Color.Lerp(nightColor, dayColor, (t - nightEnd) / transitionDuration);
    }
}