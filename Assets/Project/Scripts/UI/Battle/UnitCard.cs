using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitCard : MonoBehaviour
{
    [SerializeField] private Image unitIcon;
    [SerializeField] private TMP_Text unitName;
    [SerializeField] private TMP_Text unitPrice;
    [SerializeField] private Button button;

    private UnitDefinition definition;
    private UnitPlacementController placementController;

    public void Setup(UnitDefinition definition, UnitPlacementController placementController)
    {
        this.definition = definition;
        this.placementController = placementController;

        unitIcon.sprite = definition.icon;
        unitName.text = definition.unitName;
        unitPrice.text = definition.cost.ToString();

        button.onClick.RemoveListener(SelectUnit);
        button.onClick.AddListener(SelectUnit);
    }

    private void SelectUnit()
    {
        placementController.SelectUnit(definition);
    }
}
