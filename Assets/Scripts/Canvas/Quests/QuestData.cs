using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Quests/QuestData")]
public class QuestData : ScriptableObject
{
    [Tooltip("Quest name.")]
    public string questName;
    [TextArea]
    [Tooltip("The descriptions of the quest.")]
    public List<string> questDescs;
    [Tooltip("Quest scene names.")]
    public List<string> sceneNames;
}
