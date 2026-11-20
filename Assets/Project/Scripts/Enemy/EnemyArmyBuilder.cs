using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EnemyArmyBuilder : MonoBehaviour
{
    [Header("Generation")]

    [Header("Available Units")]
    [SerializeField] private List<UnitDefinition> unitDefinitions;

    [Header("Army Settings")]
    [SerializeField] private int goldAmount;
    [SerializeField] private int maxUnits;

    [Header("Cost Preferences")]
    // what do we consider expensive and cheap. 300 here represents that any unit >= 300 is expensive,
    // anything < will be considered cheap.
    [SerializeField] private int expensiveCheapSeperator = 300;

    [Header("Placement")]

    [Header("Placement Areas")]
    [SerializeField] private List<Collider2D> meleePlacementAreas;
    [SerializeField] private List<Collider2D> rangedPlacementAreas;

    [Header("Placement Settings")]
    [SerializeField] private LayerMask blockingLayers;
    [SerializeField] private float placementCheckRadius = 0.5f;
    [SerializeField] private int maxPlacementAttempts = 10;

    // these are relative weigths not percentages. so you don't have to have to have them all add up,
    // to same value or have between certain range. I just like to have 0-100 as its easier to visualize.
    private float meleeWeight = 60f;
    private float rangedWeight = 40f;

    private float expensiveUnitWeight = 20f;
    private float cheapUnitWeight = 80f;

    private readonly List<Unit> enemyUnits = new();

    public void CreateArmy(bool debug)
    {
        List<UnitDefinition> enemyArmy = GenerateArmy();
        List<UnitDefinition> unplacedUnits = TryPlaceArmy(enemyArmy);

        if (debug)
        {
            StringBuilder stringBuilder = new();

            stringBuilder.Append($"Generated army (size = {enemyArmy.Count}): \n");

            foreach (UnitDefinition unit in enemyArmy)
            {
                stringBuilder.Append($"Unit name: {unit.unitName}, Unit cost: {unit.cost}, Unit role: {unit.role} \n");
            }

            Debug.Log(stringBuilder.ToString());


            stringBuilder.Clear();
            stringBuilder.Append($"Unplaced units (size = {unplacedUnits.Count}): \n");

            foreach (UnitDefinition unit in unplacedUnits)
            {
                stringBuilder.Append($"Unit name: {unit.unitName}, Unit cost: {unit.cost}, Unit role: {unit.role} \n");
            }

            Debug.Log(stringBuilder.ToString());
        }
    }

    public void ClearArmy()
    {
        foreach (Unit unit in enemyUnits)
        {
            Destroy(unit.gameObject);
        }

        enemyUnits.Clear();
    }

    // ----------------- ARMY GENERATION/SELECCTION -----------------

    public List<UnitDefinition> GenerateArmy()
    {
        List<UnitDefinition> chosenUnits = new();

        int remainingGold = goldAmount;

        while (chosenUnits.Count < maxUnits)
        {
            UnitDefinition selectedUnit = SelectRandomUnit(remainingGold);

            // we will get null if no affordable units were found.
            if (selectedUnit == null) break;

            chosenUnits.Add(selectedUnit);
            remainingGold -= selectedUnit.cost;
        }

        return chosenUnits;
    }

    private UnitDefinition SelectRandomUnit(int remainingGold)
    {
        // using two lists could use hashmap. whatever unit is at index i in affordable units,
        // has a corresponding weigth in selection weights at position i.
        List<UnitDefinition> affordableUnits = new();
        List<float> selectionWeights = new();

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
        // ^1 is the same as affordableUnits.Count - 1.
        return affordableUnits[^1];
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
            return 0f;
        }
    }

    private float GetCostWeight(int cost)
    {
        if (cost >= expensiveCheapSeperator) return expensiveUnitWeight;

        return cheapUnitWeight;
    }

    // ----------------- ARMY PLACEMENT -----------------

    private List<UnitDefinition> TryPlaceArmy(List<UnitDefinition> unitsToPlace)
    {
        List<UnitDefinition> unplacedUnits = new();

        // we go through each unit and try to place it.
        foreach (UnitDefinition unitDefinition in unitsToPlace)
        {
            List<Collider2D> placementAreas = GetPlacementAreas(unitDefinition.role);

            // if for some reason there is no areas set we will skip.
            if (placementAreas == null || placementAreas.Count == 0)
            {
                unplacedUnits.Add(unitDefinition);
                continue;
            }

            // create a copy so we dont change the original inspector list.
            placementAreas = new List<Collider2D>(placementAreas);

            bool placed = false;

            // this makes it so all placement areas are equally likely of being picked.
            ShuffleAreas(placementAreas);

            foreach (Collider2D placementArea in placementAreas)
            {
                if (TryGetRandomValidPositionInPlacementArea(placementArea, out Vector2 position))
                {
                    GameObject unit = Instantiate(unitDefinition.prefab, position, Quaternion.identity);

                    Unit unitScript = unit.GetComponent<Unit>();

                    unitScript.Initialize(unitDefinition, UnitTeam.Enemy);

                    // keep track of enemy army.
                    enemyUnits.Add(unitScript);

                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                unplacedUnits.Add(unitDefinition);
            }
        }

        return unplacedUnits;
    }

    private List<Collider2D> GetPlacementAreas(UnitRole role)
    {
        if (role == UnitRole.Melee)
        {
            return meleePlacementAreas;
        }
        else if (role == UnitRole.Ranged)
        {
            return rangedPlacementAreas;
        }
        else
        {
            return null;
        }
    }

    // using out to allow us to get two outputs. i want to know if position was found and also get that position.
    private bool TryGetRandomValidPositionInPlacementArea(Collider2D placementArea, out Vector2 position)
    {
        // we only try and place a certain number of times before stopping so that we don't get stuck in infinite loop.
        for (int i = 0; i < maxPlacementAttempts; i++)
        {
            Bounds bounds = placementArea.bounds;

            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);

            // this gives us a random position inside the areas bounds,
            // however this does not guarantee that its in the actual area.
            // for a circle or polygon the bounds can be outside the area.
            Vector2 randomPosition = new Vector2(x, y);

            // making sure its actually in area.
            if (!placementArea.OverlapPoint(randomPosition)) continue;

            // making sure that its not overlapping other unit.
            Collider2D overlap = Physics2D.OverlapCircle(
                randomPosition,
                placementCheckRadius,
                blockingLayers
            );

            if (overlap != null) continue;

            position = randomPosition;
            return true;
        }

        position = new Vector2(0, 0);
        return false;
    }

    private void ShuffleAreas(List<Collider2D> areas)
    {
        for (int i = 0; i < areas.Count; i++)
        {
            int randomIndex = Random.Range(i, areas.Count);

            // swap the two areas.
            Collider2D temp = areas[i];
            areas[i] = areas[randomIndex];
            areas[randomIndex] = temp;
        }
    }

    // ---------------------- SETUP ----------------------


    public void SetRoleWeights(float melee, float ranged)
    {
        meleeWeight = melee;
        rangedWeight = ranged;
    }

    public void SetCostWeights(float expensive, float cheap)
    {
        expensiveUnitWeight = expensive;
        cheapUnitWeight = cheap;
    }

}