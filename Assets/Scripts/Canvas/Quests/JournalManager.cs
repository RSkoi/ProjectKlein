using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class JournalManager : ControllerWithWindow
{
    [Tooltip("The quest lookup table data. Associates a quest SO with a GUID.")]
    public QuestLookupData questDataForLookup;
    [Tooltip("The quest states currently tracked by the journal. Key is quest guid (lookup data).")]
    public QuestStateDictionary questStates;

    [Tooltip("The quest journal entry container")]
    public GameObject entryContainer;
    [Tooltip("Prefab template for the quests displayed in the journal.")]
    public GameObject questEntryPrefab;

    public void SetJournal(JournalCrossSceneDataType loadedQuestData)
    {
        questStates.Clear();
        foreach (QuestReferenceDataType quest in loadedQuestData.questStates)
        {
            QuestData questData = LookupQuest(quest.guid);
            questData.dayLimited = quest.dayLimited;
            questData.dayLimitedLastTick = quest.dayLimitedLastTick;
            QuestStateDataType questStateData = new(questData, quest.state);
            questStates.Add(quest.guid, questStateData);
        }
    }

    public void TrackQuest(QuestStateDataType stateData)
    {
        questStates.Add(LookupQuestGuid(stateData.quest), stateData);

        if (window.activeSelf)
            Repopulate();
    }

    public void TrackQuest(QuestStateDataType[] quests)
    {
        foreach (QuestStateDataType quest in quests)
            TrackQuest(quest);
    }

    public void UntrackQuest(string questGuid)
    {
        questStates.Remove(questGuid);
    }

    public bool QuestIsTracked(QuestData quest)
    {
        string questGuid = LookupQuestGuid(quest);
        return questStates.ContainsKey(questGuid);
    }

    public bool QuestIsTracked(QuestStateDataType questState)
    {
        string questGuid = LookupQuestGuid(questState.quest);
        return questStates.ContainsKey(questGuid);
    }

    public bool QuestIsTracked(string questGuid)
    {
        return questStates.ContainsKey(questGuid);
    }

    public JournalCrossSceneDataType PrepareJournalEntriesForSave()
    {
        List<QuestReferenceDataType> questStatesToSave = new();
        foreach (QuestStateDataType qsdt in questStates.Values)
            questStatesToSave.Add(new(
                LookupQuestGuid(qsdt.quest),
                qsdt.state,
                qsdt.quest.dayLimited,
                qsdt.quest.dayLimitedLastTick
            ));

        return new(questStatesToSave.ToArray());
    }

    public QuestData LookupQuest(string guid)
    {
        return questDataForLookup.lookupTable[guid];
    }

    public string LookupQuestGuid(QuestData quest)
    {
        return questDataForLookup.lookupTable.FirstOrDefault(x => x.Value.GetInstanceID() == quest.GetInstanceID()).Key;
    }

    public void Populate()
    {
        foreach (QuestStateDataType qsdt in questStates.Values)
        {
            GameObject entry = Instantiate(questEntryPrefab, entryContainer.transform);
            // this will potentially create problems if the prefab changes
            entry.transform.GetChild(0).GetComponent<TMP_Text>().text = qsdt.quest.questTitles[qsdt.state];
        }
    }

    public void Depopulate()
    {
        // TODO: make more efficient with pooling?
        for (int i = 0; i < entryContainer.transform.childCount; i++)
            Destroy(entryContainer.transform.GetChild(i).gameObject);
    }

    public void Repopulate()
    {
        Depopulate();
        Populate();
    }

    public override void ToggleWindow()
    {
        window.SetActive(!window.activeSelf);

        if (window.activeSelf)
            Populate();
        else
            Depopulate();
    }
}
