using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BgSongData")]
public class BgSongData : ScriptableObject
{
    [Tooltip("List of background songs.")]
    public List<AudioClip> songs;
    [Tooltip("Dialogue indexes the songs should start playing at." +
        "Indexes of this list correspond to indexes of the songs above.")]
    public List<int> indexes;
    [Tooltip("State the background song list is in. Corresponds to index of songs list.")]
    public int state = 0;
}