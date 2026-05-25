using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarUI : MonoBehaviour
{
    public GameObject hotbarSlotPrefab;
    public Transform slotsParent;
    public int hotbarSize = 8;

    private Inventory inventory;
    private HotbarSlot[] slots;
    private int selectedSlot = 0;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        inventory.OnInventoryChanged += UpdateHotbar;

        slots = new HotbarSlot[hotbarSize];
        for (int i = 0; i < hotbarSize; i++)
        {
            var obj = Instantiate(hotbarSlotPrefab, slotsParent);
            slots[i] = obj.GetComponent<HotbarSlot>();
            slots[i].slotIndex = i;
        }

        UpdateHotbar();
        HighlightSlot(0);
    }

    void Update()
    {
        // Number keys 1-8 select slots
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectSlot(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectSlot(4);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SelectSlot(5);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SelectSlot(6);
        if (Keyboard.current.digit8Key.wasPressedThisFrame) SelectSlot(7);

        // Mouse wheel cycles slots
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f) SelectSlot((selectedSlot - 1 + hotbarSize) % hotbarSize);
        if (scroll < 0f) SelectSlot((selectedSlot + 1) % hotbarSize);
    }

    void SelectSlot(int index)
    {
        selectedSlot = index;
        HighlightSlot(index);
    }

    void HighlightSlot(int index)
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].SetSelected(i == index);
    }

    void UpdateHotbar()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].UpdateSlot(inventory.items[i]);
    }

    public InventoryItem GetSelectedItem()
    {
        return inventory.items[selectedSlot];
    }
}