using System;
using UnityEngine;

[Serializable]
public class BgSongDataType
{
    public AudioClip clip;
    public int priority = 128;
    public float volume = 1; // [0, 1]
    public float pitch = 1; // [-3, 3]
    public float stereoPan = 0; // [-1, 0]
    public bool loop = true;

    public BgSongDataType(BgSongDataType prefillData)
    {
        if (prefillData != null)
        {
            clip = prefillData.clip;
            priority = prefillData.priority;
            volume = prefillData.volume;
            pitch = prefillData.pitch;
            stereoPan = prefillData.stereoPan;
            loop = prefillData.loop;
        }
    }
}
