using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemId;
    public int amount = 1;

    private SpriteRenderer sr;

    void Start()
    {
        var pid = GetComponent<PersistentID>();
        if (pid != null && CollectedPickupsTracker.Instance != null)
        {
            // destroy if already picked up in previous game
            if (CollectedPickupsTracker.Instance.IsCollected(pid.ID))
            {
                Destroy(gameObject);
                return;
            }
        }

        sr = GetComponent<SpriteRenderer>();
        var item = ItemDatabase.Instance?.GetItem(itemId);
        if (item != null && item.icon != null && sr != null)
            sr.sprite = item.icon;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger hit by: {other.gameObject.name} tag: {other.tag}");

        if (!other.CompareTag("Player")) return;
        Debug.Log("Player detected");

        var inv = other.GetComponent<Inventory>();
        if (inv == null) { Debug.LogError("No Inventory on player!"); return; }
        Debug.Log("Inventory found");

        var item = ItemDatabase.Instance?.GetItem(itemId);
        if (item == null) { Debug.LogError($"Item not found: {itemId}"); return; }
        Debug.Log($"Item found: {itemId}");

        var pid = GetComponent<PersistentID>();
        if (pid != null)
            CollectedPickupsTracker.Instance?.MarkCollected(pid.ID);

        var newItem = item.Clone();
        newItem.quantity = amount;
        inv.AddItem(newItem);

        AudioManager.Instance?.PlayPickup();

        Destroy(gameObject);
    }
}