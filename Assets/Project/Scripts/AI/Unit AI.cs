using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    public UnitDefinition unitStats;
    public State currentState = State.Idle;
    public bool holdingPosition = false;
    private Unit unit;
    private Unit target;
    private float attackTimer = 0.5f;
    public static bool IsGamePaused = false;

    private bool pathRequested = false;
    private bool hasRequestedPath = false;
    private Vector2 lastRequestedPosition = Vector2.zero;

    private float targetMoveThreshold = 1.5f;

    private Vector2 movementDestination;
    private bool hasMovementDestination = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = GetComponent<Unit>();
        unit.OnDamaged += HandleTakeDamage;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGamePaused)
        {
            unit.StopMovement();
            return;
        }

        if (unit.CurrentHealth <= 0 && currentState != State.Dead)
        {
            currentState = State.Dead;
            unit.Die();
            return;
        }   

        if (unit.IsDead)
        {
            return;
        }

        if (target != null && target.IsDead)
        {
            target = null;
            ResetTargetPath();
        }

        if (target == null && !hasMovementDestination)
        {
            FindTarget();
        }

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Hold:
                Hold();
                break;
            case State.Attacking:
                Attack();
                break;
            case State.Moving:
                Moving();
                break;

        }
    }

    public void HoldCommand()
    {
        holdingPosition = true;
        target = null;
        hasMovementDestination = false;
        ResetTargetPath();
        currentState = State.Hold;
    }

    public void AttackCommand(Unit aim)
    {
        if (aim == null || aim.Team == unit.Team)
        {
            return;
        }
        target = aim;
        hasMovementDestination = false;
        ResetTargetPath();
        holdingPosition = false;
        currentState = State.Moving;
    }

    public void CancelCommand()
    {
        holdingPosition = false;
        target = null;
        hasMovementDestination = false;
        ResetTargetPath();
        currentState = State.Idle;
    }

    public void MoveCommand(Vector2 destination)
    {
        target = null;
        holdingPosition = false;

        ResetTargetPath();

        movementDestination = destination;
        hasMovementDestination = true;

        currentState = State.Moving;
        RequestPath(destination);
    }

    void Idle()
    {
        if (target != null)
        {
            currentState = State.Moving;
        }
    }

    void Hold()
    {
        if (target != null)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance <= unitStats.range)
            {
                currentState = State.Attacking;
                unit.StopMovement();
            }
        }
    }

    private void RequestPath(Vector2 destination)
    {
        // dont request a path if one has already been requested.
        if (pathRequested) return;

        // a path must have either a target or a player movement command.
        if (target == null && !hasMovementDestination) return;

        pathRequested = true;
        hasRequestedPath = true;
        lastRequestedPosition = destination;
        bool isPlayerMovement = hasMovementDestination;

        Unit requestedTarget = target;

        PathRequestManager.RequestPath(
            transform.position,
            destination,
            // using a lambda so that we can make sure that the targets line up when the path is recieved,
            // we could ask for a path to a target and then change target before the path request gets back.
            // we also want to pass in wheter this is a player movement command.
            (waypoints, success) => OnPathFound(waypoints, success, requestedTarget, isPlayerMovement)
        );
    }

    private void OnPathFound(List<Vector2> waypoints, bool success, Unit requestedTarget, bool isPlayerMovement)
    {
        pathRequested = false;

        // dont follow the path if the unit is dead.
        // a unit can request a path and then die while its request was in queue.
        if (unit == null || unit.IsDead) return;

        // if the unit is no longer moving we can ignore the path got.
        if (currentState != State.Moving) return;

        if (!success)
        {
            hasRequestedPath = false;

            if (isPlayerMovement)
            {
                hasMovementDestination = false;
                currentState = State.Idle;
            }

            return;
        }

        // player movement command.
        if (isPlayerMovement)
        {
            if (!hasMovementDestination || target != null) return;

            unit.FollowPath(waypoints);
            return;
        }

        // targeting enemy movement.

        // ignore the path if we have changed targets.
        // or the target has died.
        if (requestedTarget == null || requestedTarget != target || requestedTarget.IsDead) return;

        unit.FollowPath(waypoints);
    }

    private void UpdatePath()
    {
        Vector2 targetPosition = target.transform.position;

        // we want to check how far the unit has moved since the last request.
        float distanceMoved = Vector2.Distance(
            lastRequestedPosition,
            targetPosition
        );

        // only requesting path if the target has moved far enough.
        if (!pathRequested && distanceMoved >= targetMoveThreshold)
        {
            RequestPath(targetPosition);
        }
    }

    private void ResetTargetPath()
    {
        hasRequestedPath = false;
        lastRequestedPosition = Vector2.zero;

        unit.StopMovement();
    }

    void Moving()
    {
        // player movement command.
        if (hasMovementDestination)
        {
            if (!unit.IsMoving && !pathRequested && hasRequestedPath)
            {
                currentState = State.Idle;
                hasMovementDestination = false;
                hasRequestedPath = false;
            }

            return;
        }

        // enemy target movement.
        if (target == null)
        {
            currentState = State.Idle;
            unit.StopMovement();
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);

        float safeRange = unitStats.range - 0.2f;

        if (distance <= safeRange)
        {
            currentState = State.Attacking;
            unit.StopMovement();
            return;
        }

        // if target is still to far away request a path to the targets position.
        // once the path has been found it will call OnPathFound and that will tell the unit to move,
        // using FollowPath in the Unit script.

        // if this is the first time requesting path to the current target we can just request one,
        // otherwise we need to update the current path.
        if (!hasRequestedPath)
        {
            RequestPath(target.transform.position);
        }
        else
        {
            UpdatePath();
        }
    }

    void Attack()
    {
        if (target == null || unit.IsDead || target.IsDead)
        {
            if (holdingPosition == true)
            {
                currentState = State.Hold;
            }
            else
            {
                currentState = State.Idle;
            }
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > unitStats.range)
        {
            if (holdingPosition == true)
            {
                currentState = State.Hold;
            }
            else
            {
                currentState = State.Moving;
            }
            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            unit.PlayAttackAnimation();
            target.TakeDamage(unitStats.damage, unit);
            attackTimer = unitStats.attack_speed;
        }
    }

    void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 10f);

        Unit nearestEnemy = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < hits.Length; i++)
        {
            Unit other = hits[i].GetComponent<Unit>();

            if (other != null && !other.IsDead && other != unit && other.Team != unit.Team)
            {
                float distance = Vector2.Distance(transform.position, other.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestEnemy = other;
                }
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy;
            ResetTargetPath();
        }
    }

    private void HandleTakeDamage(Unit attacker)
    {
        if (attacker != null && !attacker.IsDead)
        {
            float distance = Vector2.Distance(transform.position, attacker.transform.position);

            if (attacker != target && distance <= (unitStats.range + 2f))
            {
                if (currentState != State.Attacking)
                {
                    target = attacker;
                    ResetTargetPath();

                    if (holdingPosition)
                    {
                        currentState = State.Attacking;
                    }
                    else
                    {
                        currentState = State.Moving;
                    }
                }
            }
        }
    }

    public void ResetAI()
    {
        target = null;
        holdingPosition = false;
        hasMovementDestination = false;

        hasRequestedPath = false;
        lastRequestedPosition = Vector2.zero;

        currentState = State.Idle;
        unit.StopMovement();
    }
}
