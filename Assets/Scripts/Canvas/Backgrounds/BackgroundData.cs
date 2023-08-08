using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BackgroundData")]
public class BackgroundData : ScriptableObject
{
    [Tooltip("List of background textures.")]
    public List<BackgroundDataType> textures;
    [Tooltip("Dialogue indexes the backgrounds should be displayed at." +
        "Indexes of this list correspond to indexes of the textures above.")]
    public List<int> indexes;
    [Tooltip("State the background list is in. Corresponds to index of background list.")]
    public int state = 0;
}
