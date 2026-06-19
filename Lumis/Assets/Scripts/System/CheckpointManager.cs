using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public List<RobotDefinition> allRobotDefinitions;
    public GameObject robotPrefab;

    private CheckpointData savedData = null;
    private bool hasCheckpoint = false;

    void Awake() => Instance = this;

    public void SaveCheckpoint()
    {
        var player = FindFirstObjectByType<PlayerHealth>();
        var inventory = FindFirstObjectByType<Inventory>();
        var robots = FindObjectsByType<Robot>(FindObjectsSortMode.None);

        var data = new CheckpointData();

        data.playerX = player != null ? player.transform.position.x : 0f;
        data.playerY = player != null ? player.transform.position.y : 0f;

        var itemList = new List<CheckpointItem>();
        if (inventory != null)
            foreach (var item in inventory.items)
                if (item != null)
                    itemList.Add(new CheckpointItem { id = item.id, quantity = item.quantity });
        data.inventoryItems = itemList.ToArray();

        var robotList = new List<CheckpointRobot>();
        foreach (var robot in robots)
            if (robot.definition != null)
                robotList.Add(new CheckpointRobot
                {
                    definitionName = robot.definition.name,
                    x = robot.transform.position.x,
                    y = robot.transform.position.y
                });
        data.robots = robotList.ToArray();

        savedData = data;
        hasCheckpoint = true;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("Checkpoint", json);
        PlayerPrefs.SetInt("HasCheckpoint", 1);
        PlayerPrefs.Save();

        Debug.Log("Checkpoint saved");
    }

    public void LoadCheckpoint()
    {
        if (PlayerPrefs.GetInt("HasCheckpoint", 0) == 0) return;

        string json = PlayerPrefs.GetString("Checkpoint", "");
        if (string.IsNullOrEmpty(json)) return;

        savedData = JsonUtility.FromJson<CheckpointData>(json);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestoreIfNeeded()
    {
        if (savedData == null) return;

        var player = FindFirstObjectByType<PlayerHealth>();
        var inventory = FindFirstObjectByType<Inventory>();

        if (player != null)
        {
            player.transform.position = new Vector3(savedData.playerX, savedData.playerY, 0);
        }

        if (inventory != null && savedData.inventoryItems != null)
        {
            inventory.ClearAll();
            foreach (var ci in savedData.inventoryItems)
            {
                var item = ItemDatabase.Instance?.GetItem(ci.id);
                if (item == null) continue;
                var clone = item.Clone();
                clone.quantity = ci.quantity;
                inventory.AddItem(clone);
            }
        }

        if (savedData.robots != null)
            foreach (var cr in savedData.robots)
            {
                var def = allRobotDefinitions.Find(d => d.name == cr.definitionName);
                if (def == null) continue;
                var obj = Instantiate(robotPrefab,
                    new Vector3(cr.x, cr.y, 0), Quaternion.identity);
                obj.GetComponent<Robot>()?.Initialize(def);
            }

        savedData = null;
    }

    public bool HasCheckpoint() =>
        PlayerPrefs.GetInt("HasCheckpoint", 0) == 1;

    public void ClearCheckpoint()
    {
        PlayerPrefs.DeleteKey("Checkpoint");
        PlayerPrefs.DeleteKey("HasCheckpoint");
        hasCheckpoint = false;
    }
}