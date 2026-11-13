using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EnemyArmyBuilder : MonoBehaviour
{
    [Header("Available Units")]
    [SerializeField] private List<UnitDefinition> unitDefinitions;

    [Header("Army Settings")]
    [SerializeField] private int goldAmount;
    [SerializeField] private int maxUnits;

    [Header("Role Preferences")]
    // these are relative weigths not percentages. so you don't have to have to have them all add up,
    // to same value or have between certain range. I just like to have 0-100 as its easier to visualize.
    [SerializeField] private float meleeWeight = 50f;
    [SerializeField] private float rangedWeight = 40f;
    [SerializeField] private float supportWeight = 10f;

    [Header("Cost Preferences")]
    // what do we consider expensive and cheap. 300 here represents that any unit >= 300 is expensive,
    // anything < will be considered cheap.
    [SerializeField] private int expenisiveCheapSeperator = 300;
    // same as above these are not percentages.
    [SerializeField] private float expensiveUnitWeight = 20f;
    [SerializeField] private float cheapUnitWeight = 80f;

    private List<UnitDefinition> unitChoices = new List<UnitDefinition>();
    private int remainingGold;

    private void Start()
    {
        GenerateArmy();
    }

    // ----------------- ARMY GENERATION/SELECCTION -----------------

    public void GenerateArmy()
    {
        unitChoices.Clear();
        remainingGold = goldAmount;

        while (unitChoices.Count < maxUnits)
        {
            UnitDefinition selectedUnit = SelectRandomUnit();

            // we will get null if no affordable units were found.
            if (selectedUnit == null) break;

            unitChoices.Add(selectedUnit);
            remainingGold -= selectedUnit.cost;
        }

        // debug - prints chosen units.

        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append("Generated army: \n");

        foreach(UnitDefinition unit in unitChoices)
        {
            stringBuilder.Append($"Unit name: {unit.unitName}, Unit cost: {unit.cost}, Unit role: {unit.role} \n");
        }

        stringBuilder.Append($"\nRemaining gold: {remainingGold}\n");

        Debug.Log(stringBuilder.ToString());
    }

    private UnitDefinition SelectRandomUnit()
    {
        // using two lists could use hashmap. whatever unit is at index i in affordable units,
        // has a corresponding weigth in selection weights at position i.
        List<UnitDefinition> affordableUnits = new List<UnitDefinition>();
        List<float> selectionWeights = new List<float>();

        float totalWeight = 0f;

        // going through all the units to get their weights.
        foreach (UnitDefinition unit in unitDefinitions)
        {
            // just incase making sure unit definition has prefab and cost is not negative.
            if (unit.prefab == null || unit.cost <= 0) continue;

            if (unit.cost > remainingGold) continue;

            float roleWeight = GetRoleWeight(unit.role);
            float costWeight = GetCostWeight(unit.cost);

            float finalWeight = roleWeight * costWeight;

            // if we get a 0 or negative weigth we ignore this unit.
            // this should not happen but just incase something is setup wrong.
            if (finalWeight <= 0f) continue;

            affordableUnits.Add(unit);
            selectionWeights.Add(finalWeight);

            totalWeight += finalWeight;
        }

        // no units to possibly choose from.
        if (affordableUnits.Count == 0 || totalWeight <= 0f) return null;

        // pick random value. and get unit corresponding to that value.
        float randomValue = Random.Range(0f, totalWeight);

        for (int i = 0; i < affordableUnits.Count; i++)
        {
            randomValue -= selectionWeights[i];

            if (randomValue <= 0f) return affordableUnits[i];
        }

        // just incase return last unit in list.
        return affordableUnits[affordableUnits.Count - 1];
    }

    private float GetRoleWeight(UnitRole role)
    {
        if (role == UnitRole.Melee)
        {
            return meleeWeight;
        }
        else if (role == UnitRole.Ranged)
        {
            return rangedWeight;
        }
        else
        {
            return supportWeight;
        }
    }

    private float GetCostWeight(int cost)
    {
        if (cost >= expenisiveCheapSeperator) return expensiveUnitWeight;

        return cheapUnitWeight;
    }

    // 1. pick units based on amount of gold.
    // settings to decide what kind of units it prefers: 30% ranged, 30% melee, 40% support etc.
    // how much units it likes swarming or just a few strong ones.

    // 2. decide where each unit should go based on type (range/melee).
    // formation zones? (collider 2ds) decide where units are placed.
    // have few melee zones, ranged zones, support rows?
    // have formation preferences row, arrow, line etc?

    // 3. create the units and place on map
}
