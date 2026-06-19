using UnityEngine;

public class FadeTrigger : MonoBehaviour
{
    private TransparentObject parent;

    private void Awake()
    {
        parent = GetComponentInParent<TransparentObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            parent.SetTransparent(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            parent.SetTransparent(false);
    }
}