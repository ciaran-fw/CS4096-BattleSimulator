using System.Collections.Generic;
using UnityEngine;

public enum EnemyStrategy
{
    Balanced,
    CounterMelee,
    CounterRanged,
}

public class EnemyAICommander : MonoBehaviour
{
    [Header("Unit Placement Controller")]
    [SerializeField] private UnitPlacementController playerPlacementController;

    [Header("Enemy Army Builder")]
    [SerializeField] private EnemyArmyBuilder armyBuilder;

    private int meleeCount;
    private int rangedCount;

    private EnemyStrategy strategy;

    public void GenerateCounterArmy()
    {
        AnalyzePlayerArmy();
        ChooseStrategy();
        SetArmyBuilderWeights();

        armyBuilder.CreateArmy(true);

        Invoke(nameof(GiveCommand), 3f);
    }

    public void ClearArmy()
    {
        armyBuilder.ClearArmy();
    }

    private void AnalyzePlayerArmy()
    {
        // reset counts.
        meleeCount = 0;
        rangedCount = 0;

        List<Unit> playerUnits = playerPlacementController.PlacedUnits;

        foreach (Unit unit in playerUnits)
        {
            switch (unit.Definition.role)
            {
                case UnitRole.Melee:
                    meleeCount++;
                    break;
                case UnitRole.Ranged:
                    rangedCount++;
                    break;
            }
        }
    }

    private void ChooseStrategy()
    {
        if (meleeCount > rangedCount)
        {
            strategy = EnemyStrategy.CounterMelee;
        }
        else if (rangedCount > meleeCount)
        {
            strategy = EnemyStrategy.CounterRanged;
        }
        else
        {
            strategy = EnemyStrategy.Balanced;
        }
    }

    private void SetArmyBuilderWeights()
    {
        switch (strategy)
        {
            case EnemyStrategy.CounterMelee:
                armyBuilder.SetRoleWeights(20f, 70f);
                armyBuilder.SetCostWeights(20f, 80f);
                break;

            case EnemyStrategy.CounterRanged:
                armyBuilder.SetRoleWeights(70f, 20f);
                armyBuilder.SetCostWeights(80f, 20f);
                break;

            case EnemyStrategy.Balanced:
                armyBuilder.SetRoleWeights(50f, 40f);
                armyBuilder.SetCostWeights(60f, 40f);
                break;
        }
    }

    private void GiveCommand()
    {
        if (playerPlacementController.PlacedUnits.Count == 0) return;

        Unit targetPlayer = playerPlacementController.PlacedUnits[0];

        foreach (Unit enemyUnit in armyBuilder.enemyUnits)
        {
            if (enemyUnit == null) continue;

            UnitAI ai = enemyUnit.GetComponent<UnitAI>();
            if (ai != null)
            {
                if (strategy == EnemyStrategy.CounterMelee)
                {
                    ai.HoldCommand();
                }
                else
                {
                    ai.AttackCommand(targetPlayer);
                }
            }
        }
    }
}
