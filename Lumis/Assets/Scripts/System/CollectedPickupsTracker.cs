using UnityEngine;
using System.Collections.Generic;

public class CollectedPickupsTracker : MonoBehaviour
{
    public static CollectedPickupsTracker Instance;

    private HashSet<string> collectedIDs = new();
    private HashSet<string> collectedHiddenIDs = new();

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MarkCollected(string id) => collectedIDs.Add(id);
    public bool IsCollected(string id) => collectedIDs.Contains(id);
    public List<string> GetAllCollected() => new(collectedIDs);

    public void MarkCollectedHidden(string id) => collectedHiddenIDs.Add(id);
    public bool IsCollectedHidden(string id) => collectedHiddenIDs.Contains(id);
    public List<string> GetAllCollectedHidden() => new(collectedHiddenIDs);

    public void RestoreCollected(List<string> ids)
    {
        Debug.Log($"[TRACKER] RestoreCollected called with {ids.Count} IDs: {string.Join(", ", ids)}");
        collectedIDs.Clear();
        foreach (var id in ids) collectedIDs.Add(id);
    }

    public void RestoreCollectedHidden(List<string> ids)
    {
        Debug.Log($"[TRACKER] RestoreCollectedHidden called with {ids.Count} IDs");
        collectedHiddenIDs.Clear();
        foreach (var id in ids) collectedHiddenIDs.Add(id);
    }

    public void Clear()
    {
        collectedIDs.Clear();
        collectedHiddenIDs.Clear();
    }
}