using System;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    private AudioController _audioController;
    private DialogueController _dialogueController;
    private NodeManager _nodeManager;

    [Tooltip("The settings data.")]
    public SettingsData settings;

    public float maxFontSize = 1.5f;
    public float minFontSize = 0.5f;

    public Resolution[] resolutions;

    public void Awake()
    {
        resolutions = Screen.resolutions;
    }

    public void Start()
    {
        _audioController = PlayerSingleton.Instance.audioController;
        _dialogueController = PlayerSingleton.Instance.dialogueController;
        _nodeManager = PlayerSingleton.Instance.nodeManager;

        LoadAll();
    }

    public void SetFontSize(float sizeIncreaseFactor)
    {
        _dialogueController.SetFontSize(sizeIncreaseFactor);
        if (_nodeManager != null && !_nodeManager.vn)
            _nodeManager.SetFontSize(sizeIncreaseFactor);

        settings.fontSize = sizeIncreaseFactor;

        PlayerPrefs.SetFloat("fontSize", sizeIncreaseFactor);
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
        settings.resolutionWidth = width;
        settings.resolutionHeight = height;

        PlayerPrefs.SetInt("resolutionWidth", width);
        PlayerPrefs.SetInt("resolutionHeight", height);
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

        PlayerPrefs.SetFloat("volumeMusic", volume);
    }

    public void SetEffectsVolume(float volume)
    {
        _audioController.audioMixer.SetFloat(_audioController.EFFECTS_VOLUME_PARAM, Mathf.Log10(volume) * 20);
        settings.volumeEffects = volume;

        PlayerPrefs.SetFloat("volumeEffects", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        settings.fullscreen = isFullscreen;

        PlayerPrefs.SetInt("fullscreen", Convert.ToInt32(isFullscreen));
    }

    public void SetFullscreen(int isFullscreen)
    {
        SetFullscreen(Convert.ToBoolean(isFullscreen));
    }

    /*public void CurrentDefault()
    {
        SetEffectsVolume(settings.volumeEffects);
        // music is faded in to settings.volumeMusic on scene transition
        SetMusicVolume(settings.volumeMusic);
        SetFontSize(settings.fontSize);
        SetResolution(settings.resolutionWidth, settings.resolutionHeight);
        SetFullscreen(settings.fullscreen);
    }*/

    public void SaveAll()
    {
        PlayerPrefs.SetFloat("volumeMusic", settings.volumeMusic);
        PlayerPrefs.SetFloat("volumeEffects", settings.volumeEffects);
        PlayerPrefs.SetFloat("fontSize", settings.fontSize);
        PlayerPrefs.SetInt("resolutionWidth", settings.resolutionWidth);
        PlayerPrefs.SetInt("resolutionHeight", settings.resolutionHeight);
        PlayerPrefs.SetInt("fullscreen", Convert.ToInt32(Screen.fullScreen));
    }

    public void LoadAll()
    {
        SetMusicVolume(PlayerPrefs.GetFloat("volumeMusic", settings.volumeMusic));
        SetEffectsVolume(PlayerPrefs.GetFloat("volumeEffects", settings.volumeEffects));
        SetFontSize(PlayerPrefs.GetFloat("fontSize", settings.fontSize));
        SetResolution(PlayerPrefs.GetInt("resolutionWidth", settings.resolutionWidth),
            PlayerPrefs.GetInt("resolutionHeight", settings.resolutionHeight));
        SetFullscreen(PlayerPrefs.GetInt("fullscreen", Convert.ToInt32(Screen.fullScreen)));
    }
}
