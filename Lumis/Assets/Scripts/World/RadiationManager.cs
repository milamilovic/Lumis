using UnityEngine;
using System.Collections.Generic;

public class RadiationManager : MonoBehaviour
{
    public static RadiationManager Instance;

    [Header("Base radiation level (0-1)")]
    public float baseRadiation = 0.7f;

    [Header("How many nearby plants to check")]
    public int maxPlantsToCheck = 10;

    void Awake() => Instance = this;

    // Called every frame by the player to get radiation at their position
    public float GetRadiationAt(Vector2 position)
    {
        // Find all luminescent plants in scene
        var allPlants = FindObjectsByType<LuminescentPlant>(FindObjectsSortMode.None);

        // take closest plants
        List<(LuminescentPlant plant, float dist)> nearby = new();
        foreach (var plant in allPlants)
        {
            float dist = Vector2.Distance(position, plant.transform.position);
            nearby.Add((plant, dist));
        }
        nearby.Sort((a, b) => a.dist.CompareTo(b.dist));

        // Each plant reduces radiation based on its light radius and distance
        float reduction = 0f;
        int count = Mathf.Min(maxPlantsToCheck, nearby.Count);

        for (int i = 0; i < count; i++)
        {
            var (plant, dist) = nearby[i];
            float radius = plant.GetLightRadius();

            if (dist >= radius) continue;

            float falloff = 1f - (dist / radius);
            falloff = Mathf.Pow(falloff, 2f);
            reduction += plant.radiationReduction * falloff;
        }

        return Mathf.Clamp01(baseRadiation - reduction);
    }
}