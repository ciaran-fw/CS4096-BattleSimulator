using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private UnitSelectionController selectionController;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            MoveSelectedUnits();
        }
    }

    private void MoveSelectedUnits()
    {
        // get the mouse position in world coordinates.
        Vector2 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

        // get selected units.
        foreach (Unit unit in selectionController.SelectedUnits)
        {
            // just incase unit dies maybe.
            if (unit == null || unit.IsDead) continue;

            UnitAI unitAI = unit.GetComponent<UnitAI>();
            unitAI.MoveCommand(mousePosition);
        }
    }
}