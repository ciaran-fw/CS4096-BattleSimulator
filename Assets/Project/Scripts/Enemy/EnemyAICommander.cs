using System.Collections.Generic;
using UnityEngine;

public enum EnemyStrategy
{
    Balanced,
    CounterMelee,
    CounterRanged,
    CounterSupport
}

public class EnemyAICommander : MonoBehaviour
{
    [Header("Unit Placement Controller")]
    [SerializeField] private UnitPlacementController playerPlacementController;

    [Header("Enemy Army Builder")]
    [SerializeField] private EnemyArmyBuilder armyBuilder;

    private int meleeCount;
    private int rangedCount;
    private int supportCount;

    private EnemyStrategy strategy;

    public void GenerateCounterArmy()
    {
        AnalyzePlayerArmy();
        ChooseStrategy();
        SetArmyBuilderWeights();

        armyBuilder.CreateArmy(true);
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
        supportCount = 0;

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
                case UnitRole.Support:
                    supportCount++;
                    break;
            }
        }
    }

    private void ChooseStrategy()
    {
        if (meleeCount > rangedCount && meleeCount > supportCount)
        {
            strategy = EnemyStrategy.CounterMelee;
        }
        else if (rangedCount > meleeCount && rangedCount > supportCount)
        {
            strategy = EnemyStrategy.CounterRanged;
        }
        else if (supportCount > meleeCount && supportCount > rangedCount)
        {
            strategy = EnemyStrategy.CounterSupport;
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
                armyBuilder.SetRoleWeights(20f, 70f, 10f);
                break;

            case EnemyStrategy.CounterRanged:
                armyBuilder.SetRoleWeights(70f, 20f, 10f);
                break;

            case EnemyStrategy.CounterSupport:
                armyBuilder.SetRoleWeights(60f, 30f, 10f);
                break;

            case EnemyStrategy.Balanced:
                armyBuilder.SetRoleWeights(50f, 40f, 10f);
                break;
        }
    }

}
