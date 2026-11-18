using UnityEngine;

public class Unit : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Selection Indicator")]
    [SerializeField] private GameObject selectionIndicator;

    public UnitDefinition Definition { get; private set; }
    public UnitTeam Team { get; private set; }

    private Vector2 destination;
    private bool isMoving;

    private void Update()
    {
        // movement.
        if (!isMoving) return;
        MoveToDestination();
    }

    public void Initialize(UnitDefinition definition, UnitTeam team)
    {
        Definition = definition;
        Team = team;
    }

    // ---------------------- SELECTION ----------------------

    public void SetSelected(bool selected)
    {
        selectionIndicator.SetActive(selected);
    }

    // ---------------------- MOVEMENT ----------------------

    public void MoveTo(Vector2 positionToMoveTo)
    {
        destination = positionToMoveTo;
        isMoving = true;
    }

    private void MoveToDestination()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            destination,
            moveSpeed * Time.deltaTime
        );

        // if unit has reached destination stop.
        if (Vector2.Distance(transform.position, destination) < 0.05f)
        {
            transform.position = destination;
            isMoving = false;
        }
    }

    public void ResetMovement(Vector3 startingPosition)
    {
        transform.position = startingPosition;

        destination = startingPosition;
        isMoving = false;
    }
}
