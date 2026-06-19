using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public float playerX;
    public float playerY;
    public float playerHealth;

    public List<SavedItem> inventoryItems = new();

    public List<string> collectedPickupIDs = new();

    public List<SavedPlant> plants = new();

    public List<SavedRobot> robots = new();

    public int radiationSeed;
    public float radiationOffsetX;
    public float radiationOffsetY;

    public List<int> unlockedJournalOrders = new();

    public int currentDay;
    public float currentDayTime;
}

[System.Serializable]
public class SavedItem
{
    public string id;
    public int quantity;
}

[System.Serializable]
public class SavedPlant
{
    public string persistentID;
    public string definitionName;
    public float x;
    public float y;
    public int growthStage;
    public int dayAtLastGrowth;
}

[System.Serializable]
public class SavedRobot
{
    public string persistentID;
    public string definitionName;
    public float x;
    public float y;
}