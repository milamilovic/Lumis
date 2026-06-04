using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CraftingUI : MonoBehaviour
{
    [Header("Recipe list (left panel)")]
    public Transform recipeListParent;
    public GameObject recipeSlotPrefab;

    [Header("Detail panel (right)")]
    public GameObject detailPanel;
    public TextMeshProUGUI robotNameLabel;
    public Transform requirementsParent;
    public GameObject requirementRowPrefab;
    public Button craftButton;
    public TextMeshProUGUI craftButtonLabel;

    [Header("All robot definitions")]
    public List<RobotDefinition> allRobots;

    [Header("Robot prefab to spawn")]
    public GameObject robotPrefab;

    private Inventory inventory;
    public RobotDefinition selectedRecipe;

    void Start()
    {
        inventory = FindFirstObjectByType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("CraftingUI: No Inventory found in scene. Make sure the Player with Inventory component exists.");
            return;
        }
        inventory.OnInventoryChanged += RefreshCraftButton;

        foreach (var def in allRobots)
        {
            var slot = Instantiate(recipeSlotPrefab, recipeListParent)
                           .GetComponent<RecipeSlotUI>();
            slot.Setup(def, SelectRecipe);
        }

        detailPanel.SetActive(false);
    }

    public void SelectRecipe(RobotDefinition def)
    {
        selectedRecipe = def;
        detailPanel.SetActive(true);
        robotNameLabel.text = def.robotName;

        foreach (Transform child in requirementsParent)
            Destroy(child.gameObject);

        foreach (var req in def.recipe)
        {
            var row = Instantiate(requirementRowPrefab, requirementsParent);
            var labels = row.GetComponentsInChildren<TextMeshProUGUI>();
            var item = ItemDatabase.Instance?.GetItem(req.partItemId);
            int owned = CountInInventory(req.partItemId);
            labels[0].text = item != null ? item.itemName : req.partItemId;
            labels[1].text = $"{owned}/{req.amount}";
            labels[1].color = owned >= req.amount ? Color.green : Color.red;
        }

        RefreshCraftButton();
    }

    void RefreshCraftButton()
    {
        if (selectedRecipe == null) return;
        bool canCraft = CanCraft(selectedRecipe);
        craftButton.interactable = canCraft;
        craftButtonLabel.text = canCraft ? "Craft" : "Missing parts";
    }

    bool CanCraft(RobotDefinition def)
    {
        foreach (var req in def.recipe)
            if (CountInInventory(req.partItemId) < req.amount)
                return false;
        return true;
    }

    int CountInInventory(string itemId)
    {
        foreach (var item in inventory.items)
            if (item != null && item.id == itemId)
                return item.quantity;
        return 0;
    }

    public void OnCraftButtonPressed()
    {
        if (selectedRecipe == null || !CanCraft(selectedRecipe)) return;

        foreach (var req in selectedRecipe.recipe)
            inventory.RemoveItem(req.partItemId, req.amount);

        var player = FindFirstObjectByType<PlayerController>();
        Vector3 spawnPos = player.transform.position + new Vector3(1.5f, 0, 0);

        var robotObj = Instantiate(robotPrefab, spawnPos, Quaternion.identity);
        var robot = robotObj.GetComponent<Robot>();
        robot.Initialize(selectedRecipe);

        Debug.Log($"Crafted: {selectedRecipe.robotName}");
        RefreshCraftButton();

        WinScreen.Instance?.NotifyRobotCrafted();
    }
}