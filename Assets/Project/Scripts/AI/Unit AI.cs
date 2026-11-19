using UnityEngine;

public class UnitAI : MonoBehaviour
{
    public UnitDefinition unitStats;
    public State currentState = State.Idle;
    public bool holdingPosition = false;
    private Unit unit;
    private Unit target;
    private float attackTimer = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = GetComponent<Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == State.Dead)
        {
            return;
        }

        if (target == null && currentState != State.Hold)
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
        currentState = State.Hold;
    }

    public void AttackCommand(Unit aim)
    {
        if (aim == null || aim.Team == unit.Team)
        {
            return;
        }
        target = aim;
        holdingPosition = false;
        currentState = State.Moving;
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
            }
        }
    }

    void Moving()
    {
        if (target == null)
        {
            currentState = State.Idle;
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);

        float safeRange = unitStats.range - 0.2f;

        if (distance <= safeRange)
        {
            currentState = State.Attacking;
        }
        else
        {
            unit.MoveTo(target.transform.position);
        }
    }

    void Attack()
    {
        if (target == null || unit.CurrentHealth <= 0)
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
            target.TakeDamage(unitStats.damage);
            attackTimer = unitStats.attack_speed;
        }
    }

    void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 10f);

        for (int i = 0; i < hits.Length; i++)
        {
            Unit other = hits[i].GetComponent<Unit>();

            if (other != null && other != unit && other.Team != unit.Team)
            {
                target = other;
                currentState = State.Moving;
                return;
            }
        }
    }
}
