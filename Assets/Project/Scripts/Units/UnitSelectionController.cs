using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionController : MonoBehaviour
{
    [Header("Available Units")]
    [SerializeField] private List<UnitDefinition> units;

    [Header("UI")]
    [SerializeField] private UnitCard cardPrefab;
    [SerializeField] private Transform cardContainer;

    [Header("Placement")]
    [SerializeField] private UnitPlacementController placementController;

    private void Start()
    {
        CreateUnitCards();
    }

    private void CreateUnitCards()
    {
        foreach (UnitDefinition unit in units)
        {
            if (unit == null) continue;

            UnitCard card = Instantiate(cardPrefab, cardContainer);
            card.Setup(unit, placementController);
        }
    }
}
