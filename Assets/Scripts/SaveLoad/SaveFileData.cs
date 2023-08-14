using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SaveFileData")]
public class SaveFileData : ScriptableObject
{
    [Tooltip("The save data.")]
    public SaveFileDataType save;
}
