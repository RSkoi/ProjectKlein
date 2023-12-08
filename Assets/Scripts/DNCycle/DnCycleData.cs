using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DnCycleDataType")]
public class DnCycleData : ScriptableObject
{
    [Tooltip("The current data of the day/night cycle")]
    public DNCycleDataType data;
}
