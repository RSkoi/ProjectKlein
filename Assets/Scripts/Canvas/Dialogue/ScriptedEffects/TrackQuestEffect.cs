using Unity.VisualScripting;
using UnityEngine;

public class TrackQuestEffect : ScriptedEffect
{
    private JournalManager _journalManager;
    private DialogueController _dialogueController;
    private NodeManager _nodeManager;

    public QuestStateDataType questToTrack;
    public bool setsQuestState = false;
    public bool incrementTrackedQuestValue = false;

    public TrackQuestEffect()
    {
        effectName = "TrackQuestEffect";
        writesEffectText = true;
        effectTextColor = Color.yellow;
    }

    public new void InitDynamic()
    {
        effectText = effectTextIsSetByScript || effectText.Equals(DEFAULT_EFFECT_TEXT)
            ? $"Added quest: {questToTrack.quest.questName}" : effectText;

        _journalManager = PlayerSingleton.Instance.journalManager;
        if (!isNodeEffect)
            _dialogueController = PlayerSingleton.Instance.dialogueController;
        else
            _nodeManager = PlayerSingleton.Instance.nodeManager;
    }

    public new void InvokeEffect()
    {
        InitDynamic();

        if (!CheckConditions().conditionsPassed)
            return;

        if (_journalManager.QuestIsTracked(questToTrack))
        {
            QuestStateDataType questInJournal = _journalManager.questStates[questToTrack.quest.questName];
            if (setsQuestState)
                questInJournal.state = questToTrack.state;
            if (incrementTrackedQuestValue)
                questInJournal.state++;
        }
        else
        {
            QuestStateDataType newQuest = new(questToTrack.quest, setsQuestState ? questToTrack.state : 0);
            if (incrementTrackedQuestValue)
                newQuest.state++;
            _journalManager.TrackQuest(newQuest);
        }

        if (writesEffectText && !isNodeEffect)
            _dialogueController.ShowStringCombined(
                $"<color=#{effectTextColor.ToHexString()}>{effectText}</color>",
                "",
                LocalisationNamePosEnum.Middle,
                effectTextSpeed,
                false,
                0);
        else if (writesEffectTextToNode && isNodeEffect)
            _nodeManager.AddToDesc($"<color=#{effectTextColor.ToHexString()}>{effectText}</color>");
    }
}
