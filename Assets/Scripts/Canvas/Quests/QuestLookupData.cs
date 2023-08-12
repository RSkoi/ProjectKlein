using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Quests/QuestLookupData")]
public class QuestLookupData : ScriptableObject
{
    [Tooltip("The lookup table for quests. Associates a quest SO with a GUID.")]
    public QuestReferenceDictionary lookupTable;
}
