using UnityEngine;

public class ResetStatesOnStart : MonoBehaviour
{
    [Tooltip("The localisation data of this scene.")]
    public LocalisationData localization;
    [Tooltip("The background data of this scene.")]
    public BackgroundData backgrounds;
    [Tooltip("The entity data of this scene.")]
    public EntityHistoryData entityHistory;
    [Tooltip("The bg song data of this scene.")]
    public BgSongData bgSongs;
    [Tooltip("The audio effects data of this scene.")]
    public AudioEffectData audioEffects;
    [Tooltip("The particle system data of this scene.")]
    public ParticleSystemData particleSystems;

    public void Start()
    {
        Debug.Log("ResetStatesOnStart: Resetting states");

        localization.state = 0;
        backgrounds.state = 0;
        entityHistory.state = 0;
        bgSongs.state = 0;
        audioEffects.state = 0;
        particleSystems.state = 0;
    }
}
