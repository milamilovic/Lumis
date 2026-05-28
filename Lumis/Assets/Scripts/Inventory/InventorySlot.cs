using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image itemIcon;
    public TextMeshProUGUI quantityLabel;
    public Image seedBagIcon;

    [HideInInspector] public int slotIndex;
    [HideInInspector] public InventoryItem item;

    private static GameObject dragIcon;
    private static InventorySlot dragSource;
    private InventoryUI inventoryUI;
    private Canvas canvas;

    void Start()
    {
        inventoryUI = FindFirstObjectByType<InventoryUI>();
        canvas = FindFirstObjectByType<Canvas>();
    }

    public void UpdateSlot(InventoryItem newItem, int index)
    {
        item = newItem;
        slotIndex = index;

        if (item == null)
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

    public void OnBeginDrag(PointerEventData e)
    {
        if (item == null) return;

        dragSource = this;

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);
        dragIcon.transform.SetAsLastSibling();

        var rt = dragIcon.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(48, 48);

        if (item.id.Contains("seed") && seedBagIcon.sprite != null)
        {
            var bagObj = new GameObject("Bag");
            bagObj.transform.SetParent(dragIcon.transform, false);
            var bagImg = bagObj.AddComponent<Image>();
            bagImg.sprite = seedBagIcon.sprite;
            bagImg.raycastTarget = false;
            bagObj.GetComponent<RectTransform>().sizeDelta = new Vector2(48, 48);

            if (item.icon != null)
            {
                var seedObj = new GameObject("Seed");
                seedObj.transform.SetParent(dragIcon.transform, false);
                var seedImg = seedObj.AddComponent<Image>();
                seedImg.sprite = item.icon;
                seedImg.raycastTarget = false;
                seedObj.GetComponent<RectTransform>().sizeDelta = new Vector2(48, 48);
            }
        }
        else
        {
            var img = dragIcon.AddComponent<Image>();
            img.sprite = item.icon;
            img.raycastTarget = false;
        }

        itemIcon.color = new Color(1, 1, 1, 0.4f);
    }

    public void OnDrag(PointerEventData e)
    {
        if (dragIcon == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            e.position, canvas.worldCamera,
            out Vector2 localPoint);
        ((RectTransform)dragIcon.transform).localPosition = localPoint;
    }

    public void OnDrop(PointerEventData e)
    {
        if (dragSource == null || dragSource == this) return;
        inventoryUI.SwapItems(dragSource.slotIndex, slotIndex);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (dragIcon != null) Destroy(dragIcon);
        if (dragSource != null) dragSource.itemIcon.color = Color.white;
        dragSource = null;
    }
}