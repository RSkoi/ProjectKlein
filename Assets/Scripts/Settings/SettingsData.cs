using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SettingsData")]
public class SettingsData : ScriptableObject
{
    [Tooltip("Music volume.")]
    public float volumeMusic = 1f;
    [Tooltip("Effects volume.")]
    public float volumeEffects = 1f;

    [Tooltip("Font size.")]
    public float fontSize = 25f;

    [Tooltip("Fullscreen mode.")]
    public bool fullscreen = false;
    [Tooltip("Window resolution width.")]
    public int resolutionWidth = 1920;
    [Tooltip("Window resolution height.")]
    public int resolutionHeight = 1080;
}
