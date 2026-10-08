using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button button;

    private string sceneName;

    public void Setup(string levelName, string sceneName)
    {
        levelText.text = levelName;
        this.sceneName = sceneName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(LoadLevel);
    }

    private void LoadLevel()
    {
        LevelSelectManager.Instance.LoadLevel(sceneName);
    }
}
