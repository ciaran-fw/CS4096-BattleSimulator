using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int startingGold = 1500;
    [SerializeField] private int startingUnitCount = 0;

    public int StartingGold { get; private set; }
    public int StartingUnitCount { get; private set; }

    public int Gold { get; private set; }
    public int UnitCount { get; private set; }
    public float TimePlayed { get; private set; }

    public bool BattleRunning { get; private set; }
    public bool BattlePaused { get; private set; }

    private void Awake()
    {
        StartingGold = startingGold;
        StartingUnitCount = startingUnitCount;

        Gold = StartingGold;
        UnitCount = StartingUnitCount;
    }

    private void Start()
    {
        AudioManager.Instance?.PlayGameMusic();
    }

    private void Update()
    {
        if (BattleRunning && !BattlePaused)
        {
            TimePlayed += Time.deltaTime;
        }
    }

    public void PlayBattle()
    {
        BattleRunning = true;
        BattlePaused = false;
    }

    public void PauseBattle()
    {
        if (BattleRunning)
        {
            BattlePaused = true;
        }
    }

    public void StopBattle()
    {
        BattleRunning = false;
        BattlePaused = false;
        TimePlayed = 0f;
    }

    public bool SetGold(int newGold)
    {
        if (newGold < 0) return false;

        Gold = newGold;
        return true;
    }

    public bool SetUnitCount(int newUnitCount)
    {
        if (newUnitCount < 0) return false;

        UnitCount = newUnitCount;
        return true;
    }
}
