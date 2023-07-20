using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SaveFileDataType")]
public class SaveFileDataType : ScriptableObject
{
    [Tooltip("The scene name of the quicksave.")]
    public string sceneName;
    [Tooltip("The dialogue state of the quicksave.")]
    public int dialogueState;
    [Tooltip("The background state of the quicksave.")]
    public int backgroundState;
    [Tooltip("The entity state of the quicksave.")]
    public int entityState;
    // TODO: save current song?
}
