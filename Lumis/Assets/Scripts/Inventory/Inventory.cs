using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxSlots = 20;
    public InventoryItem[] items;
    public event System.Action OnInventoryChanged;

    void Awake()
    {
        items = new InventoryItem[maxSlots];
    }

    public void AddItem(InventoryItem newItem)
    {
        if (newItem == null || string.IsNullOrEmpty(newItem.id)) return;

        // Try to stack first
        for (int i = 0; i < items.Length; i++)
            if (items[i] != null && !string.IsNullOrEmpty(items[i].id) && items[i].id == newItem.id)
            {
                items[i].quantity += newItem.quantity;
                OnInventoryChanged?.Invoke();
                return;
            }

        // Find empty slot
        for (int i = 0; i < items.Length; i++)
            if (items[i] == null || string.IsNullOrEmpty(items[i].id))
            {
                items[i] = newItem;
                OnInventoryChanged?.Invoke();
                return;
            }

        Debug.LogWarning("Inventory full!");
    }

    public void RemoveItem(string id, int amount = 1)
    {
        for (int i = 0; i < items.Length; i++)
            if (items[i] != null && items[i].id == id)
            {
                items[i].quantity -= amount;
                if (items[i].quantity <= 0) items[i] = null;
                OnInventoryChanged?.Invoke();
                return;
            }
    }

    public void SwapItems(int from, int to)
    {
        if (from < 0 || from >= items.Length) return;
        if (to < 0 || to >= items.Length) return;
        (items[from], items[to]) = (items[to], items[from]);
        OnInventoryChanged?.Invoke();
    }

    public void ClearAll()
    { 
        for (int i = 0; i < items.Length; i++)
            items[i] = null;
        OnInventoryChanged?.Invoke();
    }
}