using UnityEngine;
using System.Collections.Generic;

public class CollectedPickupsTracker : MonoBehaviour
{
    public static CollectedPickupsTracker Instance;

    private HashSet<string> collectedIDs = new();

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MarkCollected(string id) => collectedIDs.Add(id);
    public bool IsCollected(string id) => collectedIDs.Contains(id);

    public List<string> GetAllCollected() => new(collectedIDs);

    public void RestoreCollected(List<string> ids)
    {
        Debug.Log($"[TRACKER] RestoreCollected called with {ids.Count} IDs: {string.Join(", ", ids)}");
        collectedIDs.Clear();
        foreach (var id in ids) collectedIDs.Add(id);
    }

    public void Clear()
    {
        collectedIDs.Clear();
    }
}