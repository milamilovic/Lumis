using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeSlotUI : MonoBehaviour
{
    public Image robotIcon;
    public TextMeshProUGUI robotNameLabel;
    public Button selectButton;

    [HideInInspector] public RobotDefinition definition;

    public void Setup(RobotDefinition def, System.Action<RobotDefinition> onSelect)
    {
        definition = def;
        robotNameLabel.text = def.robotName;
        if (def.idleSprite != null) robotIcon.sprite = def.idleSprite;
        selectButton.onClick.AddListener(() => onSelect(def));
    }
}