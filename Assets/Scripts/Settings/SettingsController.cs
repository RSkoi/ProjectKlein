using UnityEngine;

public class SettingsController : MonoBehaviour
{
    private AudioController _audioController;
    private DialogueController _dialogueController;

    [Tooltip("The settings data.")]
    public SettingsData settings;

    public float maxFontSize = 30f;
    public float minFontSize = 10f;

    public Resolution[] resolutions;

    public void Awake()
    {
        resolutions = Screen.resolutions;
    }

    public void Start()
    {
        _audioController = PlayerSingleton.Instance.audioController;
        _dialogueController = PlayerSingleton.Instance.dialogueController;

        CurrentDefault();
    }

    public void SetFontSize(float size)
    {
        if (size < minFontSize || size > maxFontSize)
            size = minFontSize;

        // TODO: this does not affect any other UI text elements
        _dialogueController.SetFontSize(size);

        settings.fontSize = size;
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
        // I think Unity already saves the set resolution somewhere in its files
        settings.resolutionWidth = width;
        settings.resolutionHeight = height;
    }

    public void SetResolutionIndex(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        SetResolution(resolution.width, resolution.height);
    }

    public void SetMusicVolume(float volume)
    {
        _audioController.audioMixer.SetFloat(_audioController.BG_SONG_VOLUME_PARAM, Mathf.Log10(volume) * 20);
        settings.volumeMusic = volume;
    }

    public void SetEffectsVolume(float volume)
    {
        _audioController.audioMixer.SetFloat(_audioController.EFFECTS_VOLUME_PARAM, Mathf.Log10(volume) * 20);
        settings.volumeEffects = volume;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        settings.fullscreen = isFullscreen;
    }

    public void CurrentDefault()
    {
        SetEffectsVolume(settings.volumeEffects);
        // music is faded in to settings.volumeMusic on scene transition
        SetMusicVolume(settings.volumeMusic);
        SetFontSize(settings.fontSize);
        SetResolution(settings.resolutionWidth, settings.resolutionHeight);
        SetFullscreen(settings.fullscreen);
    }
}
