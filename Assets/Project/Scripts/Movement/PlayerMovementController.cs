using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private UnitSelectionController selectionController;

    // we need to create callback class in this case.
    // but when we move this into UnitAI it should be easier.
    // we can call unit ai straight from the player movement controller.
    private class UnitPathCallback
    {
        private Unit unit;

        public UnitPathCallback(Unit unit)
        {
            this.unit = unit;
        }

        public void OnPathFound(List<Vector2> waypoints, bool success)
        {
            // dont move if a path couldnt be found or the unit no longer exists.
            if (!success || unit == null) return;

            // tell the unit to follow the path of waypoints.
            unit.FollowPath(waypoints);
        }
    }

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
            if (unit == null) continue;

            UnitPathCallback callback = new(unit);

            // request a path to the clicked position.
            PathRequestManager.RequestPath(unit.transform.position, mousePosition, callback.OnPathFound);
        }
    }
}