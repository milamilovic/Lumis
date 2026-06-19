using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RobotManager : MonoBehaviour
{
    public static RobotManager Instance;

    [Header("Tilemap")]
    public TileBase dugSoilTile;

    void Awake() => Instance = this;

    public List<Vector3Int> GetAllDugTiles(Tilemap dugTilemap)
    {
        var result = new List<Vector3Int>();
        if (dugTilemap == null) return result;

        BoundsInt bounds = dugTilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (dugTilemap.GetTile(pos) != null)
                result.Add(pos);
        }
        return result;
    }
}