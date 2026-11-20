using UnityEngine;

public class Unit : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [Header("Health")]
    public float CurrentHealth { get; private set; }

    [Header("Selection Indicator")]
    [SerializeField] private GameObject selectionIndicator;

    [Header("Weapon Animation")] 
    public Transform weaponPivot;
    public UnitDefinition Definition { get; private set; }
    public UnitTeam Team { get; private set; }

    private Vector2 destination;
    private bool isMoving;
    public bool IsMoving => isMoving;

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
        moveSpeed = definition.moveSpeed;
        CurrentHealth = definition.health;
    }

    // ---------------------- SELECTION ----------------------

    public void SetSelected(bool selected)
    {
        selectionIndicator.SetActive(selected);
    }

    // ---------------------- MOVEMENT ----------------------

    public void MoveTo(Vector2 positionToMoveTo)
    {
        if (CurrentHealth <= 0) return;

        destination = positionToMoveTo;
        isMoving = true;
    }

    private void MoveToDestination()
    {
        float distToDestination = Vector2.Distance(transform.position, destination);

        if (distToDestination < 1.75f)
        {
            isMoving = false;
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            destination,
            moveSpeed * Time.deltaTime
        );

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


    // ---------------------- GAMEPLAY ----------------------
    public bool IsDead { get; private set; } = false;


    public void TakeDamage(float damageAmount)
    {
        if(IsDead)
        {
            return;
        }

        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0)
        {
            IsDead = true;
        }
    }

    public void Die()
    {
        isMoving = false;
        SetSelected(false);
        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;
        transform.rotation = Quaternion.Euler(0, 0, 90f);
        Destroy(gameObject, 3f);
    }

    // ---------------------- ANIMATION ----------------------
    public void PlayAttackAnimation()
    {
        if (weaponPivot == null) return;

        weaponPivot.localScale = Vector3.one * 1.5f;

        Invoke(nameof(ResetWeapon), 0.1f);
    }

    private void ResetWeapon()
    {
        if (weaponPivot != null)
            weaponPivot.localScale = Vector3.one;
    }
}
