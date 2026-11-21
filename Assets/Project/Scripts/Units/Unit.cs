using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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


    private List<Vector2> currentWaypoints = new();
    // index of the waypoint the unit is currently moving towards.
    private int currentWaypointIndex;

    private bool isMoving;
    public bool IsMoving => isMoving;

    public event Action<Unit> OnDamaged;

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

    public void FollowPath(List<Vector2> waypoints)
    {
        if (CurrentHealth <= 0) return;

        // stopping the unit when no waypoints to follow.
        if (waypoints == null || waypoints.Count == 0)
        {
            isMoving = false;
            return;
        }

        currentWaypoints = waypoints;
        currentWaypointIndex = 0;

        isMoving = true;
    }

    public void MoveTo(Vector2 positionToMoveTo)
    {
        FollowPath(new List<Vector2> { positionToMoveTo });
    }

    private void MoveToDestination()
    {
        Vector2 currentWaypoint = currentWaypoints[currentWaypointIndex];

        float distance = Vector2.Distance(transform.position, currentWaypoint);

        // checking if we have reached the current waypoint.
        if (distance < 1f)
        {
            currentWaypointIndex++;

            // stop if reached the final waypoint.
            if (currentWaypointIndex >= currentWaypoints.Count)
            {
                isMoving = false;
                return;
            }

            // get the next waypoint.
            currentWaypoint = currentWaypoints[currentWaypointIndex];
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            currentWaypoint,
            moveSpeed * Time.deltaTime
        );
    }

    public void StopMovement()
    {
        currentWaypoints.Clear();
        currentWaypointIndex = 0;
        isMoving = false;
    }

    public void ResetMovement(Vector3 startingPosition)
    {
        transform.position = startingPosition;

        currentWaypoints.Clear();
        currentWaypointIndex = 0;
        isMoving = false;
    }

    // ---------------------- GAMEPLAY ----------------------
    public bool IsDead { get; private set; } = false;


    public void TakeDamage(float damageAmount, Unit attacker = null)
    {
        if(IsDead)
        {
            return;
        }

        OnDamaged(attacker);

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

    // ---------------------- GIZMOS ----------------------

    private void OnDrawGizmos()
    {
        if (currentWaypoints == null || currentWaypoints.Count == 0) return;

        // start drawing from the unit's current position.
        Vector3 previousPosition = transform.position;

        Gizmos.color = Color.cyan;

        // draw lines connecting the remaining waypoints.
        for (int i = currentWaypointIndex; i < currentWaypoints.Count; i++)
        {
            Vector3 waypointPosition = currentWaypoints[i];

            Gizmos.DrawLine(previousPosition, waypointPosition);
            Gizmos.DrawSphere(waypointPosition, 0.15f);

            previousPosition = waypointPosition;
        }
    }
}
