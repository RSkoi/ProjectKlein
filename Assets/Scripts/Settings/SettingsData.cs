using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SettingsData")]
public class SettingsData : ScriptableObject
{
    [Tooltip("Master volume.")]
    public float volumeMaster = 0.3f;
    [Tooltip("Music volume.")]
    public float volumeMusic = 0.5f;
    [Tooltip("Effects volume.")]
    public float volumeEffects = 0.5f;
    [Tooltip("UI Effects volume.")]
    public float volumeUIEffects = 0.5f;

    [Tooltip("Font size.")]
    public float fontSize = 1f;

    [Tooltip("Fullscreen mode.")]
    public bool fullscreen = false;
    [Tooltip("Window resolution index.")]
    public int resolutionIndex = 0;
    [Tooltip("Window resolution width.")]
    public int resolutionWidth = 1920;
    [Tooltip("Window resolution height.")]
    public int resolutionHeight = 1080;
}
