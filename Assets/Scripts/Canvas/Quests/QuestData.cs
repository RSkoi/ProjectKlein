using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Quests/QuestData")]
public class QuestData : ScriptableObject
{
    [Tooltip("Quest name.")]
    public string questName;
    [TextArea]
    [Tooltip("The titles for each section/desc of the quest.")]
    public List<string> questTitles = new();
    [TextArea]
    [Tooltip("The descriptions of the quest.")]
    public List<string> questDescs = new();
    [Tooltip("Quest scene names.")]
    public List<string> sceneNames = new();
    [Tooltip("Whether this quest can be progressed on the same day (false) or not (true)")]
    public bool dayLimited = false;
    [Tooltip("Only relevant when dayLimited is true. Day count when this quest has been last progressed")]
    public int dayLimitedLastTick = -1;
    [Tooltip("Whether this questline is still work in progress. WIP quests will not be removed from the journal.")]
    public bool wip = true;
}
