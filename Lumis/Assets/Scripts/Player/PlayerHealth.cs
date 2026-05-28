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

    public UnityEvent OnDeath;
    public UnityEvent<float> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(1f);
    }

    void Update()
    {
        if (RadiationManager.Instance == null) return;

        float radiation = RadiationManager.Instance.GetRadiationAt(transform.position);
        AudioManager.Instance?.SetRadiationVolume(radiation);

        if (radiation > safeRadiationThreshold)
        {
            float damage = damagePerSecond * radiation * Time.deltaTime;
            TakeDamage(damage);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0f)
            OnDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }
}