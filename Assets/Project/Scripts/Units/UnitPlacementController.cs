using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UnitPlacementController : MonoBehaviour
{
    [SerializeField] private Camera gameCamera;
    [SerializeField] private GameManager gameManager;

    public UnitDefinition SelectedUnit { get; private set; }

    [Header("Placement Validation")]
    [SerializeField] private LayerMask blockingLayers;
    [SerializeField] private float placementCheckRadius = 0.5f;

    [Header("Preview Colours")]
    [SerializeField] private Color validPreviewColor = new Color(0f, 1f, 0f, 0.5f);
    [SerializeField] private Color invalidPreviewColor =new Color(1f, 0f, 0f, 0.5f);

    private SpriteRenderer previewRenderer;
    private GameObject previewObject;

    private bool placementEnabled = true;

    private readonly List<Unit> placedUnits = new List<Unit>();

    private void Update()
    {
        if (!placementEnabled) return;

        UpdatePreviewPosition();

        // remove with right click.
        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
            {
                RemoveUnit();
            }
        }

        // clear all with c key.
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearPlacedUnits();
        }

        // place with left click.
        if (Input.GetMouseButtonDown(0))
        {
            if (SelectedUnit == null) return;

            // dont allow placing on ui.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            TryPlaceUnit();
        }
    }

    public void SetPlacementEnabled(bool enabled)
    {
        placementEnabled = enabled;

        if (!placementEnabled) CancelPlacement();
    }

    // ---------------------- SELECTION ------------------------------

    public void SelectUnit(UnitDefinition unit)
    {
        if (!placementEnabled) return;

        if (unit == null)
            return;

        SelectedUnit = unit;

        CreatePreview(unit);
    }

    public void CancelPlacement()
    {
        // remove selection and preview.
        if (SelectedUnit != null) SelectedUnit = null;
        if (previewObject != null) Destroy(previewObject);
    }

    // ---------------------- PLACEMENT ------------------------------

    private void TryPlaceUnit()
    {
        // if prefab doesn't exist or prefab does not have Unit script attached to it don't place.
        if (SelectedUnit.prefab == null || SelectedUnit.prefab.GetComponent<Unit>() == null) return;

        // get mouse position as position on map.
        // not fully sure how works seen online.
        // might need to change in future.
        Vector3 mousePosition = Input.mousePosition;

        mousePosition.z = -gameCamera.transform.position.z;

        Vector3 worldPosition = gameCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f;

        // checks if player has enough gold and if position is valid spot for unit.
        if (!IsPlacementValid(worldPosition))
        {
            AudioManager.Instance?.PlayError();
            return;
        }

        // spawn and initialise the unit.
        GameObject instance = Instantiate(
            SelectedUnit.prefab,
            worldPosition,
            Quaternion.identity
        );

        Unit unit = instance.GetComponent<Unit>();
        unit.Initialize(SelectedUnit);

        // add to list to keep track for removal.
        placedUnits.Add(unit);

        gameManager.SetUnitCount(gameManager.UnitCount + 1);
    }

    private bool IsPlacementValid(Vector3 position)
    {
        // player has to be able to afford the unit.
        if (gameManager == null ||
            gameManager.Gold < SelectedUnit.cost)
        {
            return false;
        }

        // checking if the position is over a collider.
        Collider2D overlap = Physics2D.OverlapCircle(
            position,
            placementCheckRadius,
            blockingLayers
        );

        return overlap == null;
    }

    // ---------------------- REMOVAL ------------------------------
    private void RemoveUnit()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -gameCamera.transform.position.z;

        Vector3 worldPosition = gameCamera.ScreenToWorldPoint(mousePosition);

        Collider2D selected = Physics2D.OverlapPoint(worldPosition);

        if (selected == null) return;

        Unit unit = selected.GetComponentInParent<Unit>();

        // just incase.
        if (unit == null || !placedUnits.Contains(unit)) return;

        placedUnits.Remove(unit);

        Destroy(unit.gameObject);

        // also just incase for some reason could go below 0.
        gameManager.SetUnitCount(Mathf.Max(0, gameManager.UnitCount - 1));
    }

    private void ClearPlacedUnits()
    {
        int removedCount = 0;

        foreach (Unit unit in placedUnits)
        {
            Destroy(unit.gameObject);
            removedCount++;
        }

        placedUnits.Clear();

        gameManager.SetUnitCount(Mathf.Max(0, gameManager.UnitCount - removedCount));
    }

    // ---------------------- PREVIEW ------------------------------

    private void CreatePreview(UnitDefinition unit)
    {
        // remove old preview.
        if (previewObject != null) Destroy(previewObject);

        previewObject = new GameObject("UnitPlacementPreview");

        previewRenderer = previewObject.AddComponent<SpriteRenderer>();
        previewRenderer.sprite = unit.icon;

        // preview should be above everything. was under map by default.
        previewRenderer.sortingOrder = 100;
        previewObject.transform.position = Vector3.zero;
    }

    private void UpdatePreviewPosition()
    {
        if (SelectedUnit == null || previewObject == null) return;

        // hide the preview while the mouse is over UI.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            previewObject.SetActive(false);
            return;
        }

        previewObject.SetActive(true);

        // get mouse position as position on map.
        // not fully sure how works seen online.
        // might need to change in future.
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -gameCamera.transform.position.z;

        Vector3 worldPosition = gameCamera.ScreenToWorldPoint(mousePosition);

        worldPosition.z = 0f;

        previewObject.transform.position = worldPosition;

        // different color depending if the unit can be placed at position or not.
        bool isPlacementValid = IsPlacementValid(worldPosition);

        if (isPlacementValid)
        {
            previewRenderer.color = validPreviewColor;
        }
        else
        {
            previewRenderer.color = invalidPreviewColor;
        }
    }
}