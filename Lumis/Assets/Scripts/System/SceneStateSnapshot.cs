using System.Collections.Generic;

public class SceneStateSnapshot
{
    public bool hasSnapshot = false;

    public int currentDay;
    public float currentDayTime;

    public int radiationSeed;
    public float radiationOffsetX;
    public float radiationOffsetY;

    public List<SavedPlant> plants = new();

    public List<SavedRobot> robots = new();

    // positions only
    public List<UnityEngine.Vector3Int> dugTiles = new();

    public List<string> collectedPickupIDs = new();
}