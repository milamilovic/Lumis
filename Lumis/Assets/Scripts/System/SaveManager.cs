using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [System.Serializable]
    public class RobotPrefabEntry
    {
        public string definitionName;
        public GameObject prefab;
    }

    [System.Serializable]
    public class PlantPrefabEntry
    {
        public string definitionName;
        public GameObject prefab;
    }

    private static bool checkpointReachedThisSession = false;
    private string SavePath => Path.Combine(Application.persistentDataPath, "checkpoint.json");
    private static bool pendingRestore = false;

    private SaveManagerConfig config;

    private static SceneStateSnapshot liveSnapshot = new SceneStateSnapshot();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        config = Resources.Load<SaveManagerConfig>("SaveManagerConfig");
        if (config == null)
            Debug.LogError("SaveManagerConfig not found in Resources folder!");
        else
            Debug.Log("SaveManagerConfig loaded successfully");
    }

    public void SaveCheckpoint()
    {
        var data = new SaveData();

        // Player
        var player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            data.playerX = player.transform.position.x;
            data.playerY = player.transform.position.y;
            data.playerHealth = player.currentHealth;
        }

        // Inventory
        var inv = FindFirstObjectByType<Inventory>();
        if (inv != null)
            foreach (var item in inv.items)
                if (item != null)
                    data.inventoryItems.Add(
                        new SavedItem { id = item.id, quantity = item.quantity });

        // Collected pickups
        data.collectedPickupIDs = CollectedPickupsTracker.Instance?.GetAllCollected()
                                  ?? new List<string>();

        // Plants
        var plants = FindObjectsByType<LuminescentPlant>(FindObjectsSortMode.None);
        foreach (var plant in plants)
        {
            var pid = plant.GetComponent<PersistentID>();
            if (pid == null) continue;
            data.plants.Add(new SavedPlant
            {
                persistentID = pid.ID,
                definitionName = plant.definition != null ? plant.definition.name : "",
                x = plant.transform.position.x,
                y = plant.transform.position.y,
                growthStage = plant.CurrentStage,
                dayAtLastGrowth = plant.DayAtLastGrowth
            });
        }

        // Robots
        var robots = FindObjectsByType<Robot>(FindObjectsSortMode.None);
        foreach (var robot in robots)
        {
            var pid = robot.GetComponent<PersistentID>();
            if (pid == null) continue;
            data.robots.Add(new SavedRobot
            {
                persistentID = pid.ID,
                definitionName = robot.definition != null ? robot.definition.name : "",
                x = robot.transform.position.x,
                y = robot.transform.position.y
            });
        }

        // Radiation seed
        if (RadiationManager.Instance != null)
        {
            data.radiationSeed = RadiationManager.Instance.noiseSeed;
            data.radiationOffsetX = RadiationManager.Instance.noiseOffsetX;
            data.radiationOffsetY = RadiationManager.Instance.noiseOffsetY;
        }

        // Journal
        // TODO

        // Day/time
        if (DayNightCycle.Instance != null)
        {
            data.currentDay = DayNightCycle.Instance.currentDay;
            data.currentDayTime = DayNightCycle.Instance.CurrentTime;
        }

        // Write to disk
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Checkpoint saved to {SavePath}");


        OnCheckpointReached();
        Debug.Log("Checkpoint saved");

        NotificationManager.Instance?.ShowNotification(
            "Checkpoint Saved", "Robot crafted - progress recorded.");
    }

    public bool HasSave() => File.Exists(SavePath);

    public void LoadCheckpoint()
    {
        Debug.Log($"LoadCheckpoint called, Instance is: {Instance}");
        
        if (!HasSave())
        {
            Debug.LogError("No save file exists!");
            return;
        }

        shouldRestoreOnLoad = true;

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager
            .LoadScene(UnityEngine.SceneManagement.SceneManager
            .GetActiveScene().buildIndex);
    }

    private static bool shouldRestoreOnLoad = false;

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene,
                       UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;

        Debug.Log("Scene loaded, restoring from save...");

        string json = System.IO.File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<SaveData>(json);
        RestoreFromData(data);

        Debug.Log("Restore complete");
    }

    public void RestoreIfPending()
    {
        Debug.Log($"RestoreIfPending called: {shouldRestoreOnLoad}");

        Debug.Log($"RestoreIfPending, shouldRestoreOnLoad={shouldRestoreOnLoad}");
        if (!shouldRestoreOnLoad) return;
        shouldRestoreOnLoad = false;

        if (!HasSave())
        {
            Debug.LogError("No save file found during restore!");
            return;
        }

        string json = System.IO.File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"Restoring: {data.plants.Count} plants, {data.robots.Count} robots");
        RestoreFromData(data);
        Debug.Log("Restore complete!");
    }

    void RestoreFromData(SaveData data)
    {
        RadiationManager.Instance?.SetSeed(
            data.radiationSeed, data.radiationOffsetX, data.radiationOffsetY);

        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.SetDay(data.currentDay, data.currentDayTime);
        }

        var player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.transform.position = new Vector3(data.playerX, data.playerY, 0);
        }

        var inv = FindFirstObjectByType<Inventory>();
        if (inv != null)
        {
            inv.ClearAll();
            foreach (var si in data.inventoryItems)
            {
                var item = ItemDatabase.Instance?.GetItem(si.id);
                if (item == null) continue;
                var clone = item.Clone();
                clone.quantity = si.quantity;
                inv.AddItem(clone);
            }
        }

        CollectedPickupsTracker.Instance?.RestoreCollected(data.collectedPickupIDs);

        foreach (var p in FindObjectsByType<LuminescentPlant>(FindObjectsSortMode.None))
            Destroy(p.gameObject);
        foreach (var r in FindObjectsByType<Robot>(FindObjectsSortMode.None))
            Destroy(r.gameObject);

        foreach (var sp in data.plants)
        {
            var def = config.allPlantDefinitions.Find(d => d.name == sp.definitionName);

            var entry = config.plantPrefabs.Find(e => e.definitionName == sp.definitionName);
            if (entry == null || entry.prefab == null)
            {
                Debug.LogWarning($"No prefab found for plant: {sp.definitionName}");
                continue;
            }

            var obj = Instantiate(entry.prefab,
                new Vector3(sp.x, sp.y, 0), Quaternion.identity);
            var plant = obj.GetComponent<LuminescentPlant>();
            if (plant != null)
            {
                plant.definition = def;
                plant.RestoreState(sp.growthStage, sp.dayAtLastGrowth);
            }
        }

        foreach (var sr in data.robots)
        {
            var def = config.allRobotDefinitions.Find(d => d.name == sr.definitionName);
            if (def == null) continue;

            var entry = config.robotPrefabs.Find(e => e.definitionName == sr.definitionName);
            if (entry == null || entry.prefab == null)
            {
                Debug.LogWarning($"No prefab found for robot: {sr.definitionName}");
                continue;
            }

            var obj = Instantiate(entry.prefab,
                new Vector3(sr.x, sr.y, 0), Quaternion.identity);
            obj.GetComponent<Robot>()?.Initialize(def);
        }

        // Journal
        // TODO
    }

    public void OnCheckpointReached()
    {
        checkpointReachedThisSession = true;
    }

    public void ResetSession()
    {
        checkpointReachedThisSession = false;
    }

    public bool ShouldRestoreOnDeath() => HasSave() && checkpointReachedThisSession;

    public void ClearSceneSnapshot()
    {
        liveSnapshot = new SceneStateSnapshot();
        Debug.Log("Scene snapshot cleared");
    }

    public void CaptureSceneSnapshot()
    {
        var snap = new SceneStateSnapshot();
        snap.hasSnapshot = true;

        if (DayNightCycle.Instance != null)
        {
            snap.currentDay = DayNightCycle.Instance.currentDay;
            snap.currentDayTime = DayNightCycle.Instance.CurrentTime;
        }

        if (RadiationManager.Instance != null)
        {
            snap.radiationSeed = RadiationManager.Instance.noiseSeed;
            snap.radiationOffsetX = RadiationManager.Instance.noiseOffsetX;
            snap.radiationOffsetY = RadiationManager.Instance.noiseOffsetY;
        }

        var plants = FindObjectsByType<LuminescentPlant>(FindObjectsSortMode.None);
        foreach (var plant in plants)
        {
            var pid = plant.GetComponent<PersistentID>();
            if (pid == null) continue;
            snap.plants.Add(new SavedPlant
            {
                persistentID = pid.ID,
                definitionName = plant.definition != null ? plant.definition.name : "",
                x = plant.transform.position.x,
                y = plant.transform.position.y,
                growthStage = plant.CurrentStage,
                dayAtLastGrowth = plant.DayAtLastGrowth
            });
        }

        var robots = FindObjectsByType<Robot>(FindObjectsSortMode.None);
        foreach (var robot in robots)
        {
            var pid = robot.GetComponent<PersistentID>();
            if (pid == null) continue;
            snap.robots.Add(new SavedRobot
            {
                persistentID = pid.ID,
                definitionName = robot.definition != null ? robot.definition.name : "",
                x = robot.transform.position.x,
                y = robot.transform.position.y
            });
        }

        var dugTilemapObj = GameObject.Find("DugGround");
        if (dugTilemapObj != null)
        {
            var tilemap = dugTilemapObj.GetComponent<UnityEngine.Tilemaps.Tilemap>();
            var robotManager = RobotManager.Instance;
            if (tilemap != null && robotManager != null)
                snap.dugTiles = robotManager.GetAllDugTiles(tilemap);
        }

        snap.collectedPickupIDs = CollectedPickupsTracker.Instance?.GetAllCollected()
                                  ?? new List<string>();

        liveSnapshot = snap;
        Debug.Log($"Scene snapshot captured: {snap.plants.Count} plants, {snap.robots.Count} robots, {snap.dugTiles.Count} dug tiles, {snap.collectedPickupIDs.Count} collected pickups");
    }

    public void RestoreSceneSnapshot()
    {

        Debug.Log($"RestoreSceneSnapshot called, hasSnapshot = {liveSnapshot?.hasSnapshot}");
        if (liveSnapshot == null || !liveSnapshot.hasSnapshot)
        {
            Debug.Log("No snapshot to restore - skipping");
            return;
        }

        var snap = liveSnapshot;

        RadiationManager.Instance?.SetSeed(snap.radiationSeed, snap.radiationOffsetX, snap.radiationOffsetY);

        DayNightCycle.Instance?.SetDay(snap.currentDay, snap.currentDayTime);

        CollectedPickupsTracker.Instance?.RestoreCollected(snap.collectedPickupIDs);

        foreach (var p in FindObjectsByType<LuminescentPlant>(FindObjectsSortMode.None))
            Destroy(p.gameObject);
        foreach (var r in FindObjectsByType<Robot>(FindObjectsSortMode.None))
            Destroy(r.gameObject);

        foreach (var sp in snap.plants)
        {
            var def = config.allPlantDefinitions.Find(d => d.name == sp.definitionName);
            var entry = config.plantPrefabs.Find(e => e.definitionName == sp.definitionName);
            if (entry == null || entry.prefab == null) continue;

            var obj = Instantiate(entry.prefab, new Vector3(sp.x, sp.y, 0), Quaternion.identity);
            var plant = obj.GetComponent<LuminescentPlant>();
            if (plant != null)
            {
                plant.definition = def;
                plant.RestoreState(sp.growthStage, sp.dayAtLastGrowth);
            }
        }

        foreach (var sr in snap.robots)
        {
            var def = config.allRobotDefinitions.Find(d => d.name == sr.definitionName);
            var entry = config.robotPrefabs.Find(e => e.definitionName == sr.definitionName);
            if (entry == null || entry.prefab == null) continue;

            var obj = Instantiate(entry.prefab, new Vector3(sr.x, sr.y, 0), Quaternion.identity);
            obj.GetComponent<Robot>()?.Initialize(def);
        }

        var dugTilemapObj = GameObject.Find("DugGround");
        if (dugTilemapObj != null)
        {
            var tilemap = dugTilemapObj.GetComponent<UnityEngine.Tilemaps.Tilemap>();
            if (tilemap != null)
                foreach (var cell in snap.dugTiles)
                    tilemap.SetTile(cell, RobotManager.Instance?.dugSoilTile);
        }

        Debug.Log("Scene snapshot restored");
    }

    public bool HasActiveSnapshotOrRestore()
    {
        return (liveSnapshot != null && liveSnapshot.hasSnapshot) || shouldRestoreOnLoad;
    }

    public void RestoreCollectedPickupsEarly()
    {
        if (liveSnapshot != null && liveSnapshot.hasSnapshot)
        {
            CollectedPickupsTracker.Instance?.RestoreCollected(liveSnapshot.collectedPickupIDs);
            Debug.Log($"[EARLY] Restored {liveSnapshot.collectedPickupIDs.Count} collected pickups in Awake");
        }
    }
}