using UnityEngine;

public class RadiationEmitter : MonoBehaviour
{
    public float radius = 5f;
    public float strength = 0.4f;
    [Range(1f, 4f)]
    public float falloffSharpness = 2f;

    void Start()
    {
        if (RadiationManager.Instance != null)
            RadiationManager.Instance.RegisterEmitter(this);
        else
            Debug.LogError("RadiationManager not found! Make sure it's in the scene.");
    }

    void OnDestroy()
    {
        if (RadiationManager.Instance != null)
            RadiationManager.Instance.UnregisterEmitter(this);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}