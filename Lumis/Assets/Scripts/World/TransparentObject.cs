using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TransparentObject : MonoBehaviour
{
    public float transparencyAmount = 0.5f;
    public float fadeSpeed = 5f;

    private SpriteRenderer sr;
    private Transform player;

    private float targetAlpha = 1f;

    private Light2D[] childLights;
    private float[] originalIntensities;
    private bool[] shouldFadeLight;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        childLights = GetComponentsInChildren<Light2D>(true);

        originalIntensities = new float[childLights.Length];
        shouldFadeLight = new bool[childLights.Length];

        for (int i = 0; i < childLights.Length; i++)
        {
            originalIntensities[i] = childLights[i].intensity;

            shouldFadeLight[i] = ShouldFadeLight(childLights[i]);
        }
    }

    bool ShouldFadeLight(Light2D light)
    {
        return light.GetComponentInParent<LuminescentPlant>() == null;
    }

    void Update()
    {
        if (player == null)
            return;

        // Fade
        Color c = sr.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
        sr.color = c;

        // Sorting
        sr.sortingOrder =
            player.position.y > transform.position.y
            ? 9999
            : -500;

        //Lights if nto plant
        if (childLights.Length > 1)
        {
            for (int i = 0; i < childLights.Length; i++)
            {
                if (!shouldFadeLight[i])
                    continue;

                float targetIntensity =
                    targetAlpha < 1f
                    ? 0.1f
                    : originalIntensities[i];

                childLights[i].intensity = Mathf.Lerp(
                    childLights[i].intensity,
                    targetIntensity,
                    fadeSpeed * Time.deltaTime
                );
            }
        }
    }

    public void SetTransparent(bool transparent)
    {
        targetAlpha = transparent ? transparencyAmount : 1f;
    }
}