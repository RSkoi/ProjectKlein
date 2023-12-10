using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Audio/AudioEffectData")]
public class AudioEffectData : ScriptableObject
{
    [Tooltip("List of audio effects.")]
    public AudioEffectDataTypeCollection[] effects = Array.Empty<AudioEffectDataTypeCollection>();
    [Tooltip("Dialogue indexes the effects should start playing at." +
        "Indexes of this list correspond to indexes of the effects above.")]
    public List<int> indexes = new();
    [Tooltip("State the effect list is in. Corresponds to index of effects list.")]
    public int state = 0;
}
