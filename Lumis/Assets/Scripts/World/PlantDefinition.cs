using UnityEngine;

[CreateAssetMenu(fileName = "NewPlant", menuName = "Game/Plant Definition")]
public class PlantDefinition : ScriptableObject
{
    public string plantName = "Luminescent Plant";

    [Header("Growth stages (3 required)")]
    public PlantStage[] stages = new PlantStage[3];

    [Header("Days between each growth stage")]
    public int daysPerStage = 3;

    [Header("Seed drop")]
    public float seedDropChancePerDay = 0.3f;
    public float seedLifetimSeconds = 30f;
    public Sprite seedSprite;
    public string seedItemId = "plant_seed";
}

[System.Serializable]
public class PlantStage
{
    public Sprite sprite;

    [Header("Light")]
    public float lightRadius = 2f;
    public float lightIntensity = 1.2f;
    public Color lightColor = new Color(0.4f, 1f, 0.5f);

    [Header("Pulse")]
    public float minBrightness = 0.8f;
    public float maxBrightness = 1.6f;
    public float pulseSpeed = 1.3f;

    [Header("Radiation")]
    public float radiationReduction = 0.2f;

    [Header("Collider")]
    public Vector2 colliderSize = new Vector2(1f, 1f);
    public Vector2 colliderOffset = Vector2.zero;
}