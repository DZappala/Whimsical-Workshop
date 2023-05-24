using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Hud : MonoBehaviour
{
    [SerializeField] private UIDocument hudDocument;
    [SerializeField] private BuildingManager buildingManager;
    private GroupBox constructionItems;

    private void Awake()
    {
        if (!hudDocument) return;
        constructionItems = hudDocument.rootVisualElement.Q<GroupBox>("ConstructionItems");
    }

    private void Start()
    {
        if (constructionItems == null) return;

        Debug.Assert(buildingManager.buildings.Count != 0, "No buildings found");

        foreach (RadioButton buildingButton in buildingManager.buildings.Select(building => new RadioButton
                     {
                         name = "ConstructionRadioButton",
                         tooltip = building.name,
                         style =
                         {
                             backgroundImage = (StyleBackground)building.iconReference.texture
                         }
                     }
                 ))
        {
            buildingButton.RegisterValueChangedCallback(evt =>
            {
                buildingButton.ToggleInClassList("buttonHighlighted");
                if (!evt.newValue) return;
                Building building = buildingManager.buildings[constructionItems.IndexOf(buildingButton)];
                building.Construct();
            });
            constructionItems.Add(buildingButton);
        }

        Debug.Assert(constructionItems.childCount != 0, "No construction items found");
    }
}