using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LocalisationData")]
public class LocalisationData : ScriptableObject
{
    [Tooltip("List of dialogue fields.")]
    public List<LocalisationDataType> dialogue = new();
    [Tooltip("List of duration fields of how long the dialogue takes to be written to the screen. " +
        "If this list is smaller than the one for dialogue strings, the first entry is seen as the default value.")]
    public List<float> dialogueSpeed = new();
    [Tooltip("State the dialogue is in. Corresponds to index of dialogue list.")]
    public int state = 0;
}
