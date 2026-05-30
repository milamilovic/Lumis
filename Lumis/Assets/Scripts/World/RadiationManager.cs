using UnityEngine;
using System.Collections.Generic;

public class RadiationManager : MonoBehaviour
{
    public static RadiationManager Instance;

    [Header("Perlin Noise Base")]
    public float noiseScale = 0.05f;
    public float noiseOffsetX = 100f;
    public float noiseOffsetY = 200f;
    public float minBaseRadiation = 0.1f;
    public float maxBaseRadiation = 0.85f;

    [Header("Plant influence")]
    public int maxPlantsToCheck = 10;

    private List<RadiationEmitter> emitters = new();

    void Awake()
    {
        Instance = this;
        noiseOffsetX = Random.Range(0f, 10000f);
        noiseOffsetY = Random.Range(0f, 10000f);
    }

    public void RegisterEmitter(RadiationEmitter e) => emitters.Add(e);
    public void UnregisterEmitter(RadiationEmitter e) => emitters.Remove(e);

    public float GetRadiationAt(Vector2 position)
    {
        float nx = position.x * noiseScale + noiseOffsetX;
        float ny = position.y * noiseScale + noiseOffsetY;
        float noise = Mathf.PerlinNoise(nx, ny); // 0-1
        float base_ = Mathf.Lerp(minBaseRadiation, maxBaseRadiation, noise);

        float emitterBoost = 0f;
        foreach (var emitter in emitters)
        {
            float dist = Vector2.Distance(position, emitter.transform.position);
            if (dist >= emitter.radius) continue;

            float t = dist / emitter.radius;
            float falloff = 1f - Mathf.SmoothStep(0f, 1f, t);
            emitterBoost += emitter.strength * falloff;
        }

        float reduction = 0f;
        var allPlants = FindObjectsByType<LuminescentPlant>(FindObjectsSortMode.None);

        List<(LuminescentPlant plant, float dist)> nearby = new();
        foreach (var plant in allPlants)
        {
            float dist = Vector2.Distance(position, plant.transform.position);
            nearby.Add((plant, dist));
        }
        nearby.Sort((a, b) => a.dist.CompareTo(b.dist));

        int count = Mathf.Min(maxPlantsToCheck, nearby.Count);
        for (int i = 0; i < count; i++)
        {
            var (plant, dist) = nearby[i];
            float radius = plant.GetLightRadius();
            if (dist >= radius) continue;
            float falloff = 1f - (dist / radius);
            falloff = Mathf.Pow(falloff, 2f);
            reduction += plant.GetRadiationReduction() * falloff;
        }

        return Mathf.Clamp01(base_ + emitterBoost - reduction);
    }

    void Update()
    {
        var player = FindFirstObjectByType<PlayerHealth>();
        //if (player != null)
            //Debug.Log($"Radiation: {GetRadiationAt(player.transform.position):F2}");
    }
}