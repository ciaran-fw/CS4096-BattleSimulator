using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", ConvertToDecibels(volume));
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", ConvertToDecibels(volume));
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", ConvertToDecibels(volume));
    }

    private float ConvertToDecibels(float value)
    {
        // prevent log10(0) from happening
        if (value <= 0.0001f)
            return -80f;

        // slider gives values from 0 - 1
        // decibels go from -80db - 0db
        return Mathf.Log10(value) * 20f;
    }
}
