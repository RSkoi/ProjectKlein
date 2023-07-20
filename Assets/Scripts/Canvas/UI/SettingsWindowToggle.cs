using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindowToggle : MonoBehaviour
{
    private SettingsController _settingsController;

    [Tooltip("The container window to toggle.")]
    public GameObject windowContainer;

    public Slider musicVolumeSlider;
    public Slider effectsVolumeSlider;
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Slider fontSizeSlider;

    public void Start()
    {
        _settingsController = PlayerSingleton.Instance.settingsController;
    }

    public void ToggleWindow()
    {
        if (windowContainer.activeSelf)
            windowContainer.SetActive(false);
        else
        {
            windowContainer.SetActive(true);
            SetCurrentSettings();
        }
    }

    public void SetCurrentSettings()
    {
        musicVolumeSlider.value = _settingsController.settings.volumeMusic;
        effectsVolumeSlider.value = _settingsController.settings.volumeEffects;

        float backupFontSize = _settingsController.settings.fontSize;
        fontSizeSlider.maxValue = _settingsController.maxFontSize;
        fontSizeSlider.minValue = _settingsController.minFontSize;
        fontSizeSlider.value = backupFontSize;
        _settingsController.settings.fontSize = backupFontSize;

        fullscreenToggle.isOn = _settingsController.settings.fullscreen;

        List<string> options = new();
        int currentResolutionIndex = 0;
        int i = 0;
        foreach (Resolution r in Screen.resolutions) {
            string option = r.width + " x " + r.height;
            options.Add(option);

            if (r.width == _settingsController.settings.resolutionWidth && r.height == _settingsController.settings.resolutionHeight)
                currentResolutionIndex = i;

            i++;
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
}
