using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnd : MonoBehaviour
{
    private bool isGameEnded = false;
    public GameObject winText;
    public GameObject loseText;
    private bool gameStart = false;

    void Start()
    {
        winText.SetActive(false);
        loseText.SetActive(false);
    }
    public void StartBattleCheck()
    {
        gameStart = true;
    }
    void Update()
    {
        if (isGameEnded || !gameStart) return;

        CheckUnits();
    }

    void CheckUnits()
    {
        int playerCount = 0;
        int enemyCount = 0;

        Unit[] allUnits = FindObjectsByType<Unit>();

        foreach (Unit unit in allUnits)
        {
            if (unit.IsDead) continue;

            if (unit.Team == UnitTeam.Player) playerCount++;
            else if (unit.Team == UnitTeam.Enemy) enemyCount++;
        }

        if (playerCount == 0 || enemyCount == 0)
            {
            if (playerCount == 0 && enemyCount == 0) return;

            if (playerCount == 0 && enemyCount != 0)
            {
                loseText.SetActive(true);
                isGameEnded = true;
                Invoke(nameof(GoToMenu), 2f);
            }
            else
            {
                winText.SetActive(true);
                isGameEnded = true;
                Invoke(nameof(GoToMenu), 2f);
            }
        }
    }

    void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ClearBattlefield()
    {
        if (winText != null) winText.SetActive(false);
        if (loseText != null) loseText.SetActive(false);


        isGameEnded = false;
        gameStart = false;
    }
}