using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string id;
    public string itemName;
    public string description;
    public int quantity = 1;
    public Sprite icon;
    public bool hideInUI = false;

    public InventoryItem Clone()
    {
        return new InventoryItem
        {
            id = id,
            itemName = itemName,
            description = description,
            quantity = quantity,
            icon = icon,
            hideInUI = hideInUI
        };
    }
}