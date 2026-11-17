using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mapSelectPanel;

    private void Start()
    {
        ShowMainMenu();
        AudioManager.Instance?.PlayMenuMusic();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        mapSelectPanel.SetActive(false);
    }

    public void ShowSettingsMenu()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        mapSelectPanel.SetActive(false);
    }

    public void ShowMapSelectMenu()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mapSelectPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMap(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).IsValid())
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log($"No scene exists with name ({sceneName})");
        }
    }
}
