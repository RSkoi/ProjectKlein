using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LocalisationData")]
public class LocalisationData : ScriptableObject
{
    [Tooltip("List of dialogue fields.")]
    public List<LocalisationDataType> dialogue;
    [Tooltip("List of speed fields the dialogue should be written at. " +
        "If this list is smaller than the one for dialogue strings, the first entry is seen as the default value.")]
    public List<float> dialogueSpeed;
    [Tooltip("State the dialogue is in. Corresponds to index of dialogue list.")]
    public int state = 0;
}
