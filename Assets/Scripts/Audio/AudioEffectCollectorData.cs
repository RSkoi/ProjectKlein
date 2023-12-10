using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Audio/AudioEffectCollectorData")]
public class AudioEffectCollectorData : ScriptableObject
{
    [Tooltip("Clips to be chosen at random to play")]
    public List<AudioClip> clips = new();
    [Tooltip("Whether the clips should be played at a semi-random rate." +
        " If false, bottom limit of random seconds is used as the rate.")]
    public bool isRandom = true;
    [Tooltip("The bottom limit of the random seconds before audio clip plays")]
    public float randomRateMin = 5f;
    [Tooltip("The upper limit of the random seconds before audio clip plays")]
    public float randomRateMax = 15f;
    [Tooltip("The bottom limit of the random stereo offset when clip plays")]
    public float randomStereoPanMin = -0.8f;
    [Tooltip("The upper limit of the random stereo offset when clip plays")]
    public float randomStereoPanMax = 0.8f;
    [Tooltip("The bottom limit of the random valume")]
    public float randomVolumeMin = 0.3f;
    [Tooltip("The upper limit of the random valume")]
    public float randomVolumeMax = 0.7f;
}
