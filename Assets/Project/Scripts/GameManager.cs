using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int startingGold = 1500;
    [SerializeField] private int startingUnitCount = 0;

    public int Gold { get; private set; }
    public int UnitCount { get; private set; }
    public float TimePlayed { get; private set; }

    public bool BattleRunning { get; private set; }
    public bool BattlePaused { get; private set; }

    private void Awake()
    {
        Gold = startingGold;
        UnitCount = startingUnitCount;
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
}
