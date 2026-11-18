using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelectionController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera gameCamera;

    [Header("Selection Box")]
    [SerializeField] private RectTransform selectionBox;
    [SerializeField] private Canvas canvas;

    [Header("Unit Layer")]
    [SerializeField] private LayerMask unitLayer;

    public readonly List<Unit> SelectedUnits = new();

    private bool selectionEnabled = false;

    private Vector2 dragStartPosition;
    private bool isDragging;


    private void Update()
    {
        if (!selectionEnabled) return;

        // start of selection.
        if (Input.GetMouseButtonDown(0))
        {
            // if mouse over ui dont allow selection.
            if (EventSystem.current.IsPointerOverGameObject()) return;

            dragStartPosition = Input.mousePosition;
            isDragging = true;

            selectionBox.gameObject.SetActive(true);
        }

        if (isDragging) UpdateSelectionBox();

        // end of selection.
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            SelectUnits();
            isDragging = false;

            selectionBox.gameObject.SetActive(false);
        }
    }

    public void SetSelectionEnabled(bool enabled)
    {
        selectionEnabled = enabled;

        if (!selectionEnabled) DeselectAllUnits();
    }

    // ------------------------------ SELECTION ------------------------------

    private void SelectUnits()
    {
        bool shiftButtonHeld = Input.GetKey(KeyCode.LeftShift);

        // if shift held we dont deselect units.
        if (!shiftButtonHeld) DeselectAllUnits();

        Vector2 mousePosition = Input.mousePosition;

        // checking if player dragged or clicked.
        if (Vector2.Distance(dragStartPosition, mousePosition) < 5f)
        {
            SelectSingleUnit(mousePosition);
        }
        else
        {
            SelectMultipleUnits(dragStartPosition, mousePosition);
        }
    }

    private void SelectSingleUnit(Vector2 position)
    {
        Vector2 worldPosition = gameCamera.ScreenToWorldPoint(position);

        Collider2D hit = Physics2D.OverlapPoint(worldPosition, unitLayer);

        if (hit == null) return;

        Unit unit = hit.GetComponentInParent<Unit>();

        if (unit.Team != UnitTeam.Player) return;

        SelectUnit(unit);
    }

    private void SelectMultipleUnits(Vector2 startPosition, Vector2 endPosition)
    {
        // got this code online from gamedevacademy.
        Vector2 worldStartPosition = gameCamera.ScreenToWorldPoint(startPosition);
        Vector2 worldEndPosition = gameCamera.ScreenToWorldPoint(endPosition);

        Vector2 centre = (worldStartPosition + worldEndPosition) / 2f;
        
        Vector2 size = new(
            Mathf.Abs(worldEndPosition.x - worldStartPosition.x),
            Mathf.Abs(worldEndPosition.y - worldStartPosition.y)
        );

        Collider2D[] overlaps = Physics2D.OverlapBoxAll(centre, size, 0f, unitLayer);

        foreach (Collider2D overlap in overlaps)
        {
            Unit unit = overlap.GetComponentInParent<Unit>();

            if (unit.Team != UnitTeam.Player) continue;

            if (!SelectedUnits.Contains(unit))
            {
                SelectUnit(unit);
            }
        }
    }


    private void SelectUnit(Unit unit)
    {
        // if unit is already in selection dont select it again.
        if (!SelectedUnits.Contains(unit))
        {
            SelectedUnits.Add(unit);
            unit.SetSelected(true);
        }
    }

    private void DeselectAllUnits()
    {
        foreach (Unit unit in SelectedUnits)
        {
            if (unit != null) unit.SetSelected(false);
        }

        SelectedUnits.Clear();
    }

    // ------------------------------ SELECTION BOX ------------------------------

    private void UpdateSelectionBox()
    {
        // this code was very similar to the multiple selection code.
        // just drwaing the box instead of selecting units.
        // used the same idea got from online.

        Vector2 currentMousePosition = Input.mousePosition;

        // get the RectTransform of the canvas.
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        // get the camera used by the canvas.
        Camera canvasCamera = canvas.worldCamera;

        Vector2 localStartPosition, localEndPosition;

        // convert the screen positions of the selection starting position and,
        // ending position to local positions on canvas.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            dragStartPosition,
            canvasCamera,
            out localStartPosition
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            currentMousePosition,
            canvasCamera,
            out localEndPosition
        );

        // calculate the bottom-left corner.
        Vector2 bottomLeft = Vector2.Min(localStartPosition, localEndPosition);

        // calculate the size.
        Vector2 size = new(
            Mathf.Abs(localEndPosition.x - localStartPosition.x),
            Mathf.Abs(localEndPosition.y - localStartPosition.y)
        );

        // account for the parents pivot position.
        selectionBox.anchoredPosition = bottomLeft - canvasRectTransform.rect.min;
        selectionBox.sizeDelta = size;
    }
}
