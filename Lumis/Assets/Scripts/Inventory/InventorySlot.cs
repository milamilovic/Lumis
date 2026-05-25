using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI quantityLabel;

    [HideInInspector] public int slotIndex;
    [HideInInspector] public InventoryItem item;

    public void UpdateSlot(InventoryItem newItem, int index)
    {
        item = newItem;
        slotIndex = index;

        if (item == null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            quantityLabel.text = "";
        }
        else
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
            quantityLabel.text = item.quantity > 1 ? item.quantity.ToString() : "";
        }
    }
}