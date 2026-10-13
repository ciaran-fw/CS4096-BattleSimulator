using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUDController : MonoBehaviour
{
    [Header("Navigation Buttons")]
    [SerializeField] private Button homeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button helpButton;

    [Header("Game Info Text")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text unitCountText;

    [Header("Popup Menus")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject helpPanel;

    [Header("Battle Control Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button stopButton;
}
