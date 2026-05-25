using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotGrid;
    public GameObject inventoryPanel;

    private Inventory inventory;
    private InventorySlot[] slots;
    private bool isOpen = false;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        inventory.OnInventoryChanged += UpdateUI;

        // Spawn all slots
        slots = new InventorySlot[inventory.maxSlots];
        for (int i = 0; i < inventory.maxSlots; i++)
        {
            var slotObj = Instantiate(slotPrefab, slotGrid);
            slots[i] = slotObj.GetComponent<InventorySlot>();
        }

        UpdateUI();
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.iKey.wasPressedThisFrame)
        {
            isOpen = !isOpen;
            inventoryPanel.SetActive(isOpen);
            if (isOpen) UpdateUI();
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].UpdateSlot(inventory.items[i], i);
    }

    public void SwapItems(int from, int to)
    {
        inventory.SwapItems(from, to);
    }
}