using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LuminescentPlant : MonoBehaviour
{
    [Header("Plant type")]
    public PlantDefinition definition;

    private int currentStage = 0;
    private int dayAtLastGrowth = 0;

    public int CurrentStage => currentStage;
    public int DayAtLastGrowth => dayAtLastGrowth;

    private bool pendingRestore = false;
    private int pendingStage = 0;
    private int pendingDay = 0;

    private SpriteRenderer sr;
    private Light2D light2D;
    private Collider2D col;

    private float pulseTime = 0f;
    private Color baseColor;

    public float transparencyAmount = 0.5f;
    public float fadeSpeed = 5f;
    private Transform player;
    private float targetAlpha = 1f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        light2D = GetComponentInChildren<Light2D>();
        col = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (pendingRestore)
        {
            pendingRestore = false;
            dayAtLastGrowth = pendingDay;
            currentStage = pendingStage;
        }

        ApplyStage(currentStage);

        if (DayNightCycle.Instance != null)
            DayNightCycle.Instance.OnNewDay += OnNewDay;
    }

    void OnDestroy()
    {
        if (DayNightCycle.Instance != null)
            DayNightCycle.Instance.OnNewDay -= OnNewDay;
    }

    void OnNewDay()
    {
        TryGrow();
        TryDropSeed();
    }

    void TryGrow()
    {
        if (definition == null) return;
        if (currentStage >= definition.stages.Length - 1) return;

        int daysSince = DayNightCycle.Instance.currentDay - dayAtLastGrowth;
        if (daysSince >= definition.daysPerStage)
        {
            currentStage++;
            dayAtLastGrowth = DayNightCycle.Instance.currentDay;
            ApplyStage(currentStage);
        }
    }

    void TryDropSeed()
    {
        if (definition == null || definition.seedSprite == null) return;
        if (Random.value > definition.seedDropChancePerDay) return;
        SpawnSeed();
    }

    void ApplyStage(int stage)
    {
        if (definition == null || stage >= definition.stages.Length) return;
        var s = definition.stages[stage];

        sr.sprite = s.sprite;
        baseColor = Color.white;

        if (light2D != null)
        {
            light2D.pointLightOuterRadius = s.lightRadius;
            light2D.intensity = s.lightIntensity;
            light2D.color = s.lightColor;
        }

        if (col != null)
        {
            col.offset = s.colliderOffset;

            if (col is CapsuleCollider2D capsule)
            {
                float width = Mathf.Max(s.colliderSize.x, 0.01f);
                float height = Mathf.Max(s.colliderSize.y, 0.01f);

                if (height >= width)
                {
                    capsule.direction = CapsuleDirection2D.Vertical;
                    capsule.size = new Vector2(width, height);
                }
                else
                {
                    capsule.direction = CapsuleDirection2D.Horizontal;
                    capsule.size = new Vector2(height, width);
                }
            }
            else if (col is BoxCollider2D box)
            {
                box.size = s.colliderSize;
            }
            else if (col is CircleCollider2D circle)
            {
                circle.radius = s.colliderSize.x;
            }
        }
    }

    void Update()
    {
        HandlePulse();
        HandleTransparency();
    }

    void HandlePulse()
    {
        float minB = definition != null ? definition.stages[currentStage].minBrightness : 0.8f;
        float maxB = definition != null ? definition.stages[currentStage].maxBrightness : 1.6f;
        float speed = definition != null ? definition.stages[currentStage].pulseSpeed : 1.3f;

        pulseTime += Time.deltaTime * speed;
        float brightness = Mathf.Lerp(minB, maxB, (Mathf.Sin(pulseTime) + 1f) / 2f);

        if (light2D != null)
            light2D.intensity = brightness;

        Color newColor = baseColor * brightness;
        newColor.a = sr.color.a;
        sr.color = newColor;
    }

    void HandleTransparency()
    {
        if (player == null) return;

        float spriteHeight = sr.bounds.size.y;
        float spriteWidth = sr.bounds.size.x;
        float objectBase = transform.position.y - spriteHeight * 0.33f;

        bool playerIsBehind = player.position.y > objectBase
                           && player.position.y < objectBase + spriteHeight * 1.5f;
        bool xOverlap = Mathf.Abs(player.position.x - transform.position.x) < spriteWidth * 0.5f;

        targetAlpha = (xOverlap && playerIsBehind) ? transparencyAmount : 1f;

        Color c = sr.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
        sr.color = c;

        sr.sortingOrder = playerIsBehind ? 9999 : -500;
    }

    public float GetLightRadius()
    {
        if (light2D != null) return light2D.pointLightOuterRadius;
        return 2f;
    }

    public float GetRadiationReduction()
    {
        if (definition != null) return definition.stages[currentStage].radiationReduction;
        return 0.3f;
    }

    void SpawnSeed()
    {
        Vector2 offset = Random.insideUnitCircle * 1.5f;
        var seedObj = new GameObject("SeedPickup");
        seedObj.transform.position = transform.position + new Vector3(offset.x, offset.y, 0);

        var seedSR = seedObj.AddComponent<SpriteRenderer>();
        seedSR.sprite = definition.seedSprite;
        seedSR.sortingLayerName = "Objects";

        var rb = seedObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;

        var seedCol = seedObj.AddComponent<CircleCollider2D>();
        seedCol.isTrigger = true;
        seedCol.radius = 0.4f;

        var pickup = seedObj.AddComponent<ItemPickup>();
        pickup.itemId = definition.seedItemId;
        pickup.amount = 1;

        seedObj.AddComponent<SeedExpiry>().lifetime = definition.seedLifetimSeconds;
        seedObj.AddComponent<SeedBob>();
    }

    public void ForceStage(int stage)
    {
        currentStage = stage;
        dayAtLastGrowth = DayNightCycle.Instance != null ? DayNightCycle.Instance.currentDay : 0;
        ApplyStage(currentStage);
    }

    public void RestoreState(int stage, int dayAtGrowth)
    {
        dayAtLastGrowth = dayAtGrowth;
        currentStage = stage;

        if (sr == null)
        {
            pendingRestore = true;
            pendingStage = stage;
            pendingDay = dayAtGrowth;
        }
        else
        {
            ApplyStage(currentStage);
        }
    }
}