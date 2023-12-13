using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : ControllerWithWindow
{
    private AudioController _audioController;
    private DialogueController _dialogueController;
    private NodeManager _nodeManager;

    [Tooltip("The settings data.")]
    public SettingsData currentSettings;
    [Tooltip("The default settings data.")]
    public SettingsData defaultSettings;

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider effectsVolumeSlider;
    public Slider uiEffectsVolumeSlider;
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Slider fontSizeSlider;

    public float maxFontSize = 1.5f;
    public float minFontSize = 0.5f;

    public Resolution[] resolutions;

    public void Awake()
    {
        resolutions = GetSupportedResolutions();
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
        if (_dialogueController != null)
            _dialogueController.SetFontSize(sizeIncreaseFactor);
        if (_nodeManager != null && !_nodeManager.vn)
            _nodeManager.SetFontSize(sizeIncreaseFactor);

        currentSettings.fontSize = sizeIncreaseFactor;

        PlayerPrefs.SetFloat("fontSize", sizeIncreaseFactor);
        PlayerPrefs.Save();
    }

    public void SetResolution(int width, int height, RefreshRate rate)
    {
        Screen.SetResolution(width, height, Screen.fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, rate);

        currentSettings.resolutionWidth = width;
        currentSettings.resolutionHeight = height;
    }

    public void SetResolutionIndex(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        SetResolution(resolution.width, resolution.height, resolution.refreshRateRatio);

        currentSettings.resolutionIndex = resolutionIndex;
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float volume)
    {
        _audioController.audioMixer.SetFloat(_audioController.MASTER_VOLUME_PARAM, Mathf.Log10(volume) * 20);

        currentSettings.volumeMaster = volume;
        PlayerPrefs.SetFloat("volumeMaster", volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        _audioController.audioMixer.SetFloat(_audioController.BG_SONG_VOLUME_PARAM, Mathf.Log10(volume) * 20);

        currentSettings.volumeMusic = volume;
        PlayerPrefs.SetFloat("volumeMusic", volume);
        PlayerPrefs.Save();
    }

    public void SetEffectsVolume(float volume)
    {
        _audioController.audioMixer.SetFloat(_audioController.EFFECTS_VOLUME_PARAM, Mathf.Log10(volume) * 20);

        currentSettings.volumeEffects = volume;
        PlayerPrefs.SetFloat("volumeEffects", volume);
        PlayerPrefs.Save();
    }

    public void SetUIEffectsVolume(float volume)
    {
        _audioController.audioMixer.SetFloat(_audioController.UI_EFFECTS_VOLUME_PARAM, Mathf.Log10(volume) * 20);

        currentSettings.volumeUIEffects = volume;
        PlayerPrefs.SetFloat("volumeUIEffects", volume);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        currentSettings.fullscreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", Convert.ToInt32(isFullscreen));
        PlayerPrefs.Save();
    }

    public void SetFullscreen(int isFullscreen)
    {
        SetFullscreen(Convert.ToBoolean(isFullscreen));
    }

    public void LoadAll()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("volumeMaster", defaultSettings.volumeMaster));
        SetMusicVolume(PlayerPrefs.GetFloat("volumeMusic", defaultSettings.volumeMusic));
        SetEffectsVolume(PlayerPrefs.GetFloat("volumeEffects", defaultSettings.volumeEffects));
        SetUIEffectsVolume(PlayerPrefs.GetFloat("volumeUIEffects", defaultSettings.volumeUIEffects));
        SetFontSize(PlayerPrefs.GetFloat("fontSize", defaultSettings.fontSize));
        SetFullscreen(PlayerPrefs.GetInt("fullscreen", Convert.ToInt32(defaultSettings.fullscreen)));
        SetResolutionIndex(PlayerPrefs.GetInt("resolutionIndex", defaultSettings.resolutionIndex));
    }

    public Resolution[] GetSupportedResolutions()
    {
        // remove non 16:9 aspect resolutions
        List<Resolution> supportedRes = new();
        foreach (Resolution r in Screen.resolutions)
            if (Math.Round((float)r.width / (float)r.height, 2) == 1.78)
                supportedRes.Add(r);
        return supportedRes.ToArray();
    }

    public void SetCurrentSettings()
    {
        masterVolumeSlider.value = currentSettings.volumeMaster;
        musicVolumeSlider.value = currentSettings.volumeMusic;
        effectsVolumeSlider.value = currentSettings.volumeEffects;
        uiEffectsVolumeSlider.value = currentSettings.volumeUIEffects;

        fontSizeSlider.maxValue = maxFontSize;
        fontSizeSlider.minValue = minFontSize;

        float currentFontSize = currentSettings.fontSize;
        fontSizeSlider.value = currentFontSize;
        currentSettings.fontSize = currentFontSize;

        fullscreenToggle.isOn = currentSettings.fullscreen;

        List<string> options = new();
        int i = 0;
        foreach (Resolution r in resolutions)
        {
            options.Add($"{r.width} x {r.height} @ {r.refreshRateRatio}");
            
            i++;
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentSettings.resolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public override void ToggleWindow()
    {
        if (window.activeSelf)
            window.SetActive(false);
        else
        {
            window.SetActive(true);
            SetCurrentSettings();
        }
    }
}
