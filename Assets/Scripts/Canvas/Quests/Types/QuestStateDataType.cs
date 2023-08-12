using System;
using UnityEngine;

[Serializable]
public class QuestStateDataType
{
    [Tooltip("The quest this state is associated with.")]
    public QuestData quest;
    [Tooltip("Quest state. Corresponds to quest desc index in QuestData.")]
    public int state;

    public QuestStateDataType(QuestData quest, int state)
    {
        this.quest = quest;
        this.state = state;
    }
}
