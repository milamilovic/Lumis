using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxSlots = 20;
    public InventoryItem[] items;
    public List<InventoryItem> hiddenItems = new();
    public event System.Action OnInventoryChanged;

    void Awake()
    {
        items = new InventoryItem[maxSlots];
    }

    public void AddItem(InventoryItem newItem)
    {
        if (newItem == null || string.IsNullOrEmpty(newItem.id)) return;

        if (newItem.hideInUI)
        {
            AddHiddenItem(newItem);
            return;
        }

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
        hiddenItems.Clear();
        OnInventoryChanged?.Invoke();
    }

    void AddHiddenItem(InventoryItem newItem)
    {
        foreach (var item in hiddenItems)
        {
            if (item.id == newItem.id)
            {
                item.quantity += newItem.quantity;
                OnInventoryChanged?.Invoke();
                return;
            }
        }
        hiddenItems.Add(newItem);
        OnInventoryChanged?.Invoke();
    }

    public int CountHiddenItem(string itemId)
    {
        foreach (var item in hiddenItems)
            if (item.id == itemId)
                return item.quantity;
        return 0;
    }

    public void RemoveHiddenItem(string itemId, int amount = 1)
    {
        for (int i = 0; i < hiddenItems.Count; i++)
        {
            if (hiddenItems[i].id == itemId)
            {
                hiddenItems[i].quantity -= amount;
                if (hiddenItems[i].quantity <= 0)
                    hiddenItems.RemoveAt(i);
                OnInventoryChanged?.Invoke();
                return;
            }
        }
    }
}