using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private AudioManager audioManager;

    [Header("UI References")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    private void Start()
    {
        audioManager = AudioManager.Instance;

        SetupListeners();
        LoadSettings();
    }

    public void SetMasterVolume(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        audioManager.SetMasterVolume(value);

        audioManager.PlayButtonHover();

        SaveSettings();
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        audioManager.SetMusicVolume(value);

        audioManager.PlayButtonHover();

        SaveSettings();
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        audioManager.SetSFXVolume(value);

        audioManager.PlayButtonHover();

        SaveSettings();
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;

        if (value == true)
        {
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Fullscreen", 0);
        }

        audioManager.PlayButtonClick();

        SaveSettings();
    }

    private void SetupListeners()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void LoadSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        masterVolumeSlider.SetValueWithoutNotify(masterVolume);
        musicVolumeSlider.SetValueWithoutNotify(musicVolume);
        sfxVolumeSlider.SetValueWithoutNotify(sfxVolume);
        fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

        audioManager.SetMasterVolume(masterVolume);
        audioManager.SetMusicVolume(musicVolume);
        audioManager.SetSFXVolume(sfxVolume);

        Screen.fullScreen = fullscreen;
    }

    private void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}
