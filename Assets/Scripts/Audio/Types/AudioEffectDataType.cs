using System;
using UnityEngine;

[Serializable]
public class AudioEffectDataType
{
    public AudioClip clip;
    public int priority = 127;
    public float volume; // [0, 1]
    public float pitch = 1; // [-3, 3]
    public float stereoPan; // [-1, 0]
    public bool loop;
}
