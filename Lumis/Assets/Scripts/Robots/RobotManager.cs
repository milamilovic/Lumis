using UnityEngine;
using UnityEngine.Tilemaps;

public class RobotManager : MonoBehaviour
{
    public static RobotManager Instance;

    [Header("Tilemap")]
    public TileBase dugSoilTile;

    void Awake() => Instance = this;
}