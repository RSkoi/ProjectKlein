using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FlagData")]
public class FlagData : ScriptableObject
{
    [Tooltip("List of currently active flags.")]
    public List<FlagDataType> flags;
}