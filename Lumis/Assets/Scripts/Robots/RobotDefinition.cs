using UnityEngine;

public enum RobotType { Digger, Planter }

[CreateAssetMenu(fileName = "NewRobot", menuName = "Game/Robot Definition")]
public class RobotDefinition : ScriptableObject
{
    public string robotName;
    public RobotType type;
    public Sprite idleSprite;
    public RuntimeAnimatorController animatorController;

    [Header("Task")]
    public float secondsPerTile = 3f;
    public float moveSpeed = 2f;

    [Header("Crafting recipe")]
    public RobotPartRequirement[] recipe;
}

[System.Serializable]
public class RobotPartRequirement
{
    public string partItemId;
    public int amount;
}