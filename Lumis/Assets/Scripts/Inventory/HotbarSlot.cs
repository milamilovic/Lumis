using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarSlot : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI quantityLabel;
    public Image selectionHighlight;

    [HideInInspector] public int slotIndex;

    public void UpdateSlot(InventoryItem item)
    {
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