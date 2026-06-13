using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class TaskAssignmentManager : MonoBehaviour
{
    public static TaskAssignmentManager Instance;

    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap dugTilemap;
    public TileBase dugSoilTile;

    [Header("Selection box")]
    public LineRenderer selectionBox;

    [Header("Assignment Panel")]
    public GameObject assignmentPanel;
    public TextMeshProUGUI robotNameLabel;
    public TextMeshProUGUI instructionLabel;
    public Button confirmButton;
    public Button cancelButton;

    [Header("Seed Popup")]
    public GameObject seedPopup;
    public Transform seedOptionParent;
    public GameObject seedOptionButtonPrefab;

    [Header("Seed options")]
    public List<SeedOption> allSeedOptions;

    private Robot selectedRobot;
    private bool isSelectingArea = false;
    private bool isDragging = false;
    private Vector3 dragStart;
    private Vector3 dragEnd;
    private List<Vector3Int> selectedTiles = new();
    private SeedOption chosenSeed = null;

    private Camera cam;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    void Start()
    {
        assignmentPanel.SetActive(false);
        seedPopup.SetActive(false);
        if (selectionBox != null) selectionBox.enabled = false;

        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(CancelAssignment);
    }

    public void SelectRobot(Robot robot)
    {
        selectedRobot = robot;
        selectedRobot.StopWandering();
        chosenSeed = null;
        selectedTiles.Clear();

        Time.timeScale = 0f;

        // Show panel
        assignmentPanel.SetActive(true);
        seedPopup.SetActive(false);
        robotNameLabel.text = robot.definition.robotName;
        instructionLabel.text = "Drag to select tiles";
        confirmButton.gameObject.SetActive(false);

        isSelectingArea = true;
    }

    void Update()
    {
        if (selectedRobot == null) return;

        Vector3 worldMouse = GetWorldMousePos();

        if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUI())
        {
            dragStart = worldMouse;
            isDragging = true;
            if (selectionBox != null) selectionBox.enabled = true;
        }

        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            dragEnd = worldMouse;
            if (selectionBox != null) selectionBox.enabled = true;
            DrawSelectionBox(dragStart, dragEnd);
        }

        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            dragEnd = worldMouse;
            isDragging = false;

            selectedTiles = GetTilesInRect(dragStart, dragEnd);
            instructionLabel.text = $"{selectedTiles.Count} tiles selected";

            if (selectedTiles.Count > 0)
            {
                if (selectedRobot.definition.type == RobotType.Planter)
                    ShowSeedPopup();
                else
                    confirmButton.gameObject.SetActive(true);
            }
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            CancelAssignment();
    }

    void ShowSeedPopup()
    {
        seedPopup.SetActive(true);
        confirmButton.gameObject.SetActive(false);

        foreach (Transform child in seedOptionParent)
            Destroy(child.gameObject);

        var inventory = FindFirstObjectByType<Inventory>();

        foreach (var seed in allSeedOptions)
        {
            int owned = CountInInventory(inventory, seed.seedItemId);
            bool enough = owned >= selectedTiles.Count;

            var btn = Instantiate(seedOptionButtonPrefab, seedOptionParent);

            var icon = btn.transform.Find("Icon")?.GetComponent<Image>();
            if (icon != null && seed.icon != null) icon.sprite = seed.icon;

            var label = btn.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
            if (label != null)
                label.text = $"{seed.displayName}\n{owned} owned / {selectedTiles.Count} needed";

            var button = btn.GetComponent<Button>();
            button.interactable = enough;

            if (label != null)
                label.color = enough ? Color.white : new Color(0.5f, 0.5f, 0.5f);

            // choose on click
            var capturedSeed = seed;
            button.onClick.AddListener(() => ChooseSeed(capturedSeed));
        }
    }

    void ChooseSeed(SeedOption seed)
    {
        chosenSeed = seed;
        seedPopup.SetActive(false);
        instructionLabel.text = $"{selectedTiles.Count} tiles — planting {seed.displayName}";
        confirmButton.gameObject.SetActive(true);
    }

    void OnConfirm()
    {
        if (selectedRobot == null || selectedTiles.Count == 0) return;

        // planting robot decreases seed quantity
        if (selectedRobot.definition.type == RobotType.Planter)
        {
            if (chosenSeed == null) return;

            var inventory = FindFirstObjectByType<Inventory>();
            inventory.RemoveItem(chosenSeed.seedItemId, selectedTiles.Count);
            selectedRobot.plantPrefab = chosenSeed.plantPrefab;
        }

        selectedRobot.AssignTiles(selectedTiles);
        ClosePanel();
    }

    public void CancelAssignment()
    {
        if (selectedRobot != null)
            selectedRobot.StartWandering();
        ClosePanel();
    }

    void ClosePanel()
    {
        selectedRobot = null;
        selectedTiles.Clear();
        chosenSeed = null;
        isDragging = false;
        isSelectingArea = false;

        if (selectionBox != null) selectionBox.enabled = false;

        assignmentPanel.SetActive(false);
        seedPopup.SetActive(false);
        if (selectionBox != null) selectionBox.enabled = false;

        // Unpause
        Time.timeScale = 1f;
    }

    int CountInInventory(Inventory inv, string itemId)
    {
        if (inv == null) return 0;
        foreach (var item in inv.items)
            if (item != null && item.id == itemId)
                return item.quantity;
        return 0;
    }

    List<Vector3Int> GetTilesInRect(Vector3 a, Vector3 b)
    {
        var tiles = new List<Vector3Int>();
        if (groundTilemap == null) return tiles;

        Vector3Int cellA = groundTilemap.WorldToCell(a);
        Vector3Int cellB = groundTilemap.WorldToCell(b);

        int minX = Mathf.Min(cellA.x, cellB.x);
        int maxX = Mathf.Max(cellA.x, cellB.x);
        int minY = Mathf.Min(cellA.y, cellB.y);
        int maxY = Mathf.Max(cellA.y, cellB.y);

        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
                tiles.Add(new Vector3Int(x, y, 0));

        return tiles;
    }

    void DrawSelectionBox(Vector3 a, Vector3 b)
    {
        if (selectionBox == null) return;
        selectionBox.enabled = true;
        selectionBox.useWorldSpace = true;
        selectionBox.positionCount = 5;
        selectionBox.SetPositions(new Vector3[]
        {
            new Vector3(a.x, a.y, 0),
            new Vector3(b.x, a.y, 0),
            new Vector3(b.x, b.y, 0),
            new Vector3(a.x, b.y, 0),
            new Vector3(a.x, a.y, 0)
        });
    }

    Vector3 GetWorldMousePos()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 pos = cam.ScreenToWorldPoint(screenPos);
        pos.z = 0f;
        return pos;
    }

    bool IsPointerOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
}