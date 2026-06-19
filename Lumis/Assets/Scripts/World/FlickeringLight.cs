using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickeringLight : MonoBehaviour
{
    [Header("Intensity range")]
    public float onIntensity = 1.2f;
    public float offIntensity = 0f;

    [Header("Flicker timing")]
    public float minOnDuration = 1.5f;
    public float maxOnDuration = 4f;
    public float minOffDuration = 0.05f;
    public float maxOffDuration = 0.2f;

    [Header("Occasional longer blackout")]
    public float chanceForLongBlackout = 0.05f;
    public float longBlackoutDuration = 1f;

    private Light2D light2D;
    private bool isOn = true;
    private float nextSwitchTime;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        light2D.intensity = onIntensity;

        // Randomize starting offset so lights don't sync
        nextSwitchTime = Time.time + Random.Range(0f, maxOnDuration);
    }

    void Update()
    {
        if (light2D == null) return;

        if (Time.time >= nextSwitchTime)
        {
            isOn = !isOn;
            light2D.intensity = isOn ? onIntensity : offIntensity;

            if (isOn)
            {
                nextSwitchTime = Time.time + Random.Range(minOnDuration, maxOnDuration);
            }
            else
            {
                bool longBlackout = Random.value < chanceForLongBlackout;
                float offDuration = longBlackout
                    ? longBlackoutDuration
                    : Random.Range(minOffDuration, maxOffDuration);

                nextSwitchTime = Time.time + offDuration;
            }
        }
    }
}