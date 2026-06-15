[System.Serializable]
public class CheckpointData
{
    public float playerX;
    public float playerY;

    // Inventory snapshot
    public CheckpointItem[] inventoryItems;

    // Robots that existed at checkpoint
    public CheckpointRobot[] robots;
}

[System.Serializable]
public class CheckpointItem
{
    public string id;
    public int quantity;
}

[System.Serializable]
public class CheckpointRobot
{
    public string definitionName;
    public float x;
    public float y;
}