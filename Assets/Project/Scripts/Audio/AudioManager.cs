using UnityEngine;
using UnityEngine.Audio;

// resposible for music and sfx only not game sounds as they should be positional.
// should probaly separate into two managers in future one for music and another for ui sfx.

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource uiSFXSource;

    [Header("UI Sound Effects")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip buttonHover;
    [SerializeField] private AudioClip panelOpen;
    [SerializeField] private AudioClip error;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;

    [Header("UI Sound Pitch")]
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudioSources();
    }

    private void SetupAudioSources()
    {
        // setup audio sources incase they are wrong in inspector.
        musicSource.outputAudioMixerGroup = musicGroup;
        uiSFXSource.outputAudioMixerGroup = sfxGroup;

        musicSource.playOnAwake = false;
        uiSFXSource.playOnAwake = false;

        musicSource.loop = true;

        uiSFXSource.spatialBlend = 0f;
        musicSource.spatialBlend = 0f;
    }

    // volume.

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

    // sounds.

    private void PlayUISound(AudioClip clip)
    {
        if (clip == null || uiSFXSource == null)
            return;

        uiSFXSource.pitch = Random.Range(
            pitchRange.x,
            pitchRange.y
        );

        uiSFXSource.PlayOneShot(clip);
    }

    public void PlayButtonClick()
    {
        PlayUISound(buttonClick);
    }

    public void PlayButtonHover()
    {
        PlayUISound(buttonHover);
    }

    public void PlayPanelOpen()
    {
        PlayUISound(panelOpen);
    }

    public void PlayError()
    {
        PlayUISound(error);
    }

    // music.

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null) return;

        musicSource.Stop();
        musicSource.clip = null;
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayGameMusic()
    {
        PlayMusic(gameMusic);
    }
}
