using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;

    [Header("Unit Placement and Selection")]
    [SerializeField] private GameObject unitSelectionPanel;
    [SerializeField] private GameObject sideDivider;
    [SerializeField] private UnitPlacementController placementController;
    [SerializeField] private UnitSelectionController selectionController;

    [Header("Camera")]
    [SerializeField] private CameraMovement camera;

    [Header("Enemy AI")]
    [SerializeField] private EnemyAICommander enemyCommander;

    private Dictionary<Unit, Vector3> startingPositions = new();

    // ---------------------- BATTLE CONTROL ----------------------

    public void PlayBattle()
    {
        // if game is paused just resume battle.
        if (gameManager.BattlePaused)
        {
            gameManager.PlayBattle();
            return;
        }

        if (gameManager.BattleRunning) return;

        RecordStartingPositions();

        enemyCommander.GenerateCounterArmy();

        // turn unit placement off and unit selection on.
        placementController.SetPlacementEnabled(false);
        selectionController.SetSelectionEnabled(true);

        // hide divider and unit selection panel when game starts.
        sideDivider.SetActive(false);
        unitSelectionPanel.SetActive(false);

        gameManager.PlayBattle();
    }

    public void PauseBattle()
    {
        gameManager.PauseBattle();
    }

    public void StopBattle()
    {
        gameManager.StopBattle();

        ResetStartingPositions();

        // turn unit placement on and unit selection off.
        placementController.SetPlacementEnabled(true);
        selectionController.SetSelectionEnabled(false);

        camera.ResetCamera();

        // show divider and unit selection panel when game ends.
        sideDivider.SetActive(true);
        unitSelectionPanel.SetActive(true);

        enemyCommander.ClearArmy();
    }

    // ---------------------- ARMY PERSISTANCE ----------------------

    private void RecordStartingPositions()
    {
        startingPositions.Clear();

        foreach (Unit unit in placementController.PlacedUnits)
        {
            if (unit == null) continue;

            startingPositions.Add(unit, unit.transform.position);
        }
    }

    private void ResetStartingPositions()
    {
        foreach (KeyValuePair<Unit, Vector3> pair in startingPositions)
        {
            Unit unit = pair.Key;

            unit.ResetMovement(pair.Value);
        }
    }
}
