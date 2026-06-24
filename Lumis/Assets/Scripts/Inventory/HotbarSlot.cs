using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarSlot : MonoBehaviour
{
    public Image itemIcon;
    public Image seedBagIcon;
    public TextMeshProUGUI quantityLabel;
    public Image selectionHighlight;

    [HideInInspector] public int slotIndex;

    public void UpdateSlot(InventoryItem item)
    {
        if (item != null && string.IsNullOrEmpty(item.id))
            item = null;

        if (item == null || item.hideInUI)
        {
            if (itemIcon != null) { itemIcon.sprite = null; itemIcon.enabled = false; }
            if (seedBagIcon != null) seedBagIcon.enabled = false;
            if (quantityLabel != null) quantityLabel.text = "";
        }
        else if (item.id.Contains("seed"))
        {
            if (itemIcon != null) { itemIcon.sprite = item.icon; itemIcon.enabled = item.icon != null; }
            if (seedBagIcon != null) seedBagIcon.enabled = true;
            if (quantityLabel != null) quantityLabel.text = item.quantity > 1 ? item.quantity.ToString() : "";
        }
        else
        {
            if (itemIcon != null) { itemIcon.sprite = item.icon; itemIcon.enabled = item.icon != null; }
            if (seedBagIcon != null) seedBagIcon.enabled = false;
            if (quantityLabel != null) quantityLabel.text = item.quantity > 1 ? item.quantity.ToString() : "";
        }
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight != null)
        {
            // Bright white tint when selected, dimmed when not
            selectionHighlight.color = selected
                ? new Color(0.5f, 0.5f, 0.5f, 0.0f)
                : new Color(0.5f, 0.5f, 0.5f, 0.7f);
        }
    }
}