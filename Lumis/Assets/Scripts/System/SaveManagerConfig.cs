using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SaveManagerConfig", menuName = "Game/Save Manager Config")]
public class SaveManagerConfig : ScriptableObject
{
    public List<RobotDefinition> allRobotDefinitions;
    public List<PlantDefinition> allPlantDefinitions;
    public List<SaveManager.RobotPrefabEntry> robotPrefabs;
    public List<SaveManager.PlantPrefabEntry> plantPrefabs;
}