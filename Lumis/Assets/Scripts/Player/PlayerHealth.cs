using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }

    [Header("Radiation damage")]
    public float damagePerSecond = 10f;       // at radiation = 1.0
    public float safeRadiationThreshold = 0.2f; // no damage bellow

    [HideInInspector] 
    public bool isIndoors { get; set; } = false;

    public UnityEvent OnDeath;
    public UnityEvent<float> OnHealthChanged;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        OnDeath.AddListener(() =>
        {
            Debug.Log("OnDeath listener fired");
            LoseScreen.Instance?.Show();
        });
        OnHealthChanged?.Invoke(1f);
    }

    void Update()
    {
        if (RadiationManager.Instance == null) return;

        var player = FindFirstObjectByType<PlayerHealth>();

        if (player != null && player.isIndoors)
        {
            float radiation = 0f; // force to zero indoors
            AudioManager.Instance?.SetRadiationVolume(0f);
        }
        else if (RadiationManager.Instance != null && player != null)
        {
            float radiation = RadiationManager.Instance.GetRadiationAt(player.transform.position);
            AudioManager.Instance?.SetRadiationVolume(radiation);

            if (radiation > safeRadiationThreshold)
            {
                float damage = damagePerSecond * radiation * Time.deltaTime;
                TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0f)
        {
            isDead = true;
            Debug.Log("Player died, invoking OnDeath");
            OnDeath?.Invoke();
        }
    }

    public void Heal()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(maxHealth);
    }
}