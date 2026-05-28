using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider;
    public Image fillImage;

    public Color fullHealthColor = new Color(0.2f, 0.9f, 0.3f);
    public Color lowHealthColor = new Color(0.9f, 0.2f, 0.2f);

    void Start()
    {
        var health = FindFirstObjectByType<PlayerHealth>();
        if (health != null)
            health.OnHealthChanged.AddListener(SetHealth);
    }

    public void SetHealth(float normalized)
    {
        healthSlider.value = normalized;
        fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, normalized);
    }
}