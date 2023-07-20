using UnityEngine;

public class ResetStatesOnStart : MonoBehaviour
{
    [Tooltip("The localisation data of this scene.")]
    public LocalisationData localization;
    [Tooltip("The background data of this scene.")]
    public BackgroundData backgrounds;
    [Tooltip("The entity data of this scene.")]
    public EntityHistoryData entityHistory;

    public void Awake()
    {
        localization.state = 0;
        backgrounds.state = 0;
        entityHistory.state = 0;
    }
}
