using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance { get; private set; }

    [Serializable]
    public class LevelData
    {
        public string levelName;
        public string sceneName;
    }

    [Header("Level Button")]
    [SerializeField] private LevelButton levelButtonPrefab;
    [SerializeField] private Transform levelButtonParent;

    [Header("Levels")]
    [SerializeField] private LevelData[] levels;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateLevelButtons();
    }

    private void CreateLevelButtons()
    {
        foreach (LevelData level in levels)
        {
            LevelButton button = Instantiate(
                levelButtonPrefab,
                levelButtonParent
            );

            button.Setup(level.levelName, level.sceneName);
        }
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

