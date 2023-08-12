using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class JournalManager : MonoBehaviour
{
    [Tooltip("The quest lookup table data. Associates a quest SO with a GUID.")]
    public QuestLookupData questDataForLookup;
    [Tooltip("The quest states currently tracked by the journal. Key is quest name")]
    public QuestStateDictionary questStates;

    [Tooltip("The quest journal window container")]
    public GameObject window;
    [Tooltip("The quest journal entry container")]
    public GameObject entryContainer;
    [Tooltip("Prefab template for the quests displayed in the journal.")]
    public GameObject questEntryPrefab;

    public void TrackQuest(string questName, QuestStateDataType stateData)
    {
        questStates.Add(questName, stateData);

        if (window.activeSelf)
            Repopulate();
    }

    public void UntrackQuest(string questName)
    {
        questStates.Remove(questName);
    }

    public bool QuestIsTracked(QuestData quest)
    {
        return questStates.ContainsKey(quest.questName);
    }

    public bool QuestIsTracked(QuestStateDataType questState)
    {
        return questStates.ContainsKey(questState.quest.questName);
    }

    public bool QuestIsTracked(string questName)
    {
        return questStates.ContainsKey(questName);
    }

    public JournalCrossSceneDataType PrepareJournalEntriesForSave()
    {
        List<QuestReferenceDataType> questStatesToSave = new();
        foreach (QuestStateDataType qsdt in questStates.Values)
            questStatesToSave.Add(new(
                questDataForLookup.lookupTable.FirstOrDefault(x => x.Value.GetInstanceID() == qsdt.quest.GetInstanceID()).Key,
                qsdt.state
            ));

        return new(questStatesToSave.ToArray());
    }

    public QuestData LookupQuest(string guid)
    {
        return questDataForLookup.lookupTable[guid];
    }

    public void ToggleWindow()
    {
        window.SetActive(!window.activeSelf);

        if (window.activeSelf)
            Populate();
        else 
            Depopulate();
    }

    public void Populate()
    {
        foreach (QuestStateDataType qsdt in questStates.Values)
        {
            GameObject entry = Instantiate(questEntryPrefab, entryContainer.transform);
            // this will potentially create problems if the prefab changes
            entry.transform.GetChild(0).GetComponent<TMP_Text>().text = qsdt.quest.questName;
        }
    }

    public void Depopulate()
    {
        // TODO: make more efficient?
        for (int i = 0; i < entryContainer.transform.childCount; i++)
            Destroy(entryContainer.transform.GetChild(i).gameObject);
    }

    public void Repopulate()
    {
        Depopulate();
        Populate();
    }
}
