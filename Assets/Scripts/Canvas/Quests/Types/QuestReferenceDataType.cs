using System;
using UnityEngine;

[Serializable]
public class QuestReferenceDataType
{
    [Tooltip("The guid of this quest.")]
    public string guid;
    [Tooltip("The state this quest is in.")]
    public int state;

    public QuestReferenceDataType(string guid, int state)
    {
        this.guid = guid;
        this.state = state;
    }
}
