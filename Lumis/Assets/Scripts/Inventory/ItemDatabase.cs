using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public List<ItemIconEntry> iconMappings;

    private Dictionary<string, InventoryItem> items = new();

    [System.Serializable]
    public class ItemIconEntry
    {
        public string id;
        public Sprite icon;
    }

    void Awake()
    {
        Instance = this;
        LoadItems();
    }

    void LoadItems()
    {
        var json = Resources.Load<TextAsset>("Data/items_data");
        if (json == null) { Debug.LogError("items_data.json not found in Resources/Data/"); return; }

        var parsed = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ItemEntry>>(json.text);
        if (parsed == null) return;

        foreach (var kvp in parsed)
        {
            var item = new InventoryItem
            {
                id = kvp.Key,
                itemName = kvp.Value.name,
                description = kvp.Value.description
            };

            var iconEntry = iconMappings?.Find(e => e.id == kvp.Key);
            if (iconEntry != null) item.icon = iconEntry.icon;

            items[kvp.Key] = item;
        }

        Debug.Log($"Loaded {items.Count} items from database");
    }

    public InventoryItem GetItem(string id) =>
        items.TryGetValue(id, out var item) ? item : null;
}

[System.Serializable]
class ItemEntry
{
    public string name;
    public string description;
}