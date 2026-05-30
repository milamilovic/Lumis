using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TaskAssignmentManager : MonoBehaviour
{
    public static TaskAssignmentManager Instance;

    [Header("Selection box UI (world space)")]
    public LineRenderer selectionBox;

    [Header("Assignment panel UI")]
    public GameObject assignmentPanel;
    public TextMeshProUGUI robotNameLabel;
    public TextMeshProUGUI instructionLabel;
    public Button cancelButton;

    public GameObject planterSeedPanel;
    public TMP_Dropdown seedDropdown;
    public List<string> availableSeedIds;
    public List<GameObject> availablePlantPrefabs;

    private Robot selectedRobot;
    private bool isSelectingArea = false;
    private Vector3 dragStart;
    private Vector3 dragEnd;
    private Camera cam;
    private Tilemap groundTilemap;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;
        groundTilemap = GameObject.Find("Ground")?.GetComponent<Tilemap>();
    }

    void Start()
    {
        assignmentPanel.SetActive(false);
        if (selectionBox != null) selectionBox.enabled = false;
        cancelButton.onClick.AddListener(CancelAssignment);
    }

    public void SelectRobot(Robot robot)
    {
        if (Time.timeScale == 0f) return;

        selectedRobot = robot;
        isSelectingArea = false;

        assignmentPanel.SetActive(true);
        robotNameLabel.text = robot.definition.robotName;
        instructionLabel.text = "Drag to select area";

        bool isPlanter = robot.definition.type == RobotType.Planter;
        if (planterSeedPanel != null)
            planterSeedPanel.SetActive(isPlanter);
    }

    void Update()
    {
        if (selectedRobot == null) return;

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            dragStart = GetWorldMousePos();
            isSelectingArea = true;
        }

        if (isSelectingArea && Input.GetMouseButton(0))
        {
            dragEnd = GetWorldMousePos();
            DrawSelectionBox(dragStart, dragEnd);
        }

        if (isSelectingArea && Input.GetMouseButtonUp(0))
        {
            dragEnd = GetWorldMousePos();
            if (selectionBox != null) selectionBox.enabled = false;

            var tiles = GetTilesInRect(dragStart, dragEnd);
            AssignTaskToRobot(tiles);
            isSelectingArea = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelAssignment();
    }

    void AssignTaskToRobot(List<Vector3Int> tiles)
    {
        if (tiles.Count == 0) return;

        if (selectedRobot.definition.type == RobotType.Planter)
        {
            int idx = seedDropdown != null ? seedDropdown.value : 0;
            if (idx < availablePlantPrefabs.Count)
                selectedRobot.plantPrefab = availablePlantPrefabs[idx];
        }

        selectedRobot.AssignTiles(tiles);
        instructionLabel.text = $"Assigned {tiles.Count} tiles";
        assignmentPanel.SetActive(false);
        selectedRobot = null;
    }

    void CancelAssignment()
    {
        selectedRobot = null;
        isSelectingArea = false;
        assignmentPanel.SetActive(false);
        if (selectionBox != null) selectionBox.enabled = false;
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
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        return pos;
    }

    bool IsPointerOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
}