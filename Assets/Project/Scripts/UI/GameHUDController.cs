using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHUDController : MonoBehaviour
{
    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;

    [Header("Navigation Buttons")]
    [SerializeField] private Button homeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button helpButton;

    [Header("Game Info Text")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text unitCountText;

    [Header("Popups")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private GameObject pausedOverlay;

    [Header("Battle Control Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button stopButton;

    [Header("Unit Placement")]
    [SerializeField] private GameObject unitSelectionPanel;
    [SerializeField] private UnitPlacementController placementController;

    [Header("Camera")]
    [SerializeField] private CameraMovement camera;

    private void Awake()
    {
        homeButton.onClick.AddListener(GoHome);
        settingsButton.onClick.AddListener(ToggleSettings);
        helpButton.onClick.AddListener(ToggleHelp);

        playButton.onClick.AddListener(PlayBattle);
        pauseButton.onClick.AddListener(PauseBattle);
        stopButton.onClick.AddListener(StopBattle);

        settingsPanel.SetActive(false);
        helpPanel.SetActive(false);

        UpdateInformation();
        UpdateBattleButtons();
    }

    private void Update()
    {
        if (gameManager == null)
        {
            Debug.Log("Game manager not set!");
            return;
        }

        UpdateInformation();
        UpdateBattleButtons();
    }

    private void UpdateInformation()
    {
        goldText.text = gameManager.Gold.ToString();
        unitCountText.text = gameManager.UnitCount.ToString();


        int totalSeconds = Mathf.FloorToInt(gameManager.TimePlayed);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    private void UpdateBattleButtons()
    {
        bool running = gameManager.BattleRunning;
        bool paused = gameManager.BattlePaused;

        // play can be pressed when stopped or paused.
        playButton.interactable = !running || paused;

        // pause can be pressed only while running.
        pauseButton.interactable = running && !paused;

        // stop can be pressed while running or paused.
        stopButton.interactable = running || paused;

        pausedOverlay.SetActive(paused);
    }


    private void GoHome()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ToggleSettings()
    {
        bool showSettings = !settingsPanel.activeSelf;

        settingsPanel.SetActive(showSettings);

        AudioManager.Instance?.PlayPanelOpen();

        if (showSettings)
        {
            helpPanel.SetActive(false);
        }
    }

    private void ToggleHelp()
    {
        bool showHelp = !helpPanel.activeSelf;

        helpPanel.SetActive(showHelp);
        AudioManager.Instance?.PlayPanelOpen();

        if (showHelp)
        {
            settingsPanel.SetActive(false);
        }
    }

    private void PlayBattle()
    {
        gameManager.PlayBattle();

        unitSelectionPanel.SetActive(false);
        placementController.SetPlacementEnabled(false);
    }

    private void PauseBattle()
    {
        gameManager.PauseBattle();
    }

    private void StopBattle()
    {
        gameManager.StopBattle();

        unitSelectionPanel.SetActive(true);
        placementController.SetPlacementEnabled(true);

        camera.ResetCamera();
    }
}
