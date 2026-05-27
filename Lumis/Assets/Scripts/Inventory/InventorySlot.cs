using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image itemIcon;
    public TextMeshProUGUI quantityLabel;

    [HideInInspector] public int slotIndex;
    [HideInInspector] public InventoryItem item;

    private static GameObject dragIcon;
    private static InventorySlot dragSource;
    private InventoryUI inventoryUI;
    private Canvas canvas;

    void Start()
    {
        inventoryUI = GetComponentInParent<InventoryUI>();
        canvas = GetComponentInParent<Canvas>();
    }

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

    public void OnBeginDrag(PointerEventData e)
    {
        if (item == null) return;

        dragSource = this;

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);
        dragIcon.transform.SetAsLastSibling();

        var img = dragIcon.AddComponent<Image>();
        img.sprite = item.icon;
        img.raycastTarget = false;

        var rt = dragIcon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(48, 48);

        itemIcon.color = new Color(1, 1, 1, 0.4f); // dim the source slot
    }

    public void OnDrag(PointerEventData e)
    {
        if (dragIcon == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            e.position, canvas.worldCamera,
            out Vector2 localPoint);
        dragIcon.GetComponent<RectTransform>().localPosition = localPoint;
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