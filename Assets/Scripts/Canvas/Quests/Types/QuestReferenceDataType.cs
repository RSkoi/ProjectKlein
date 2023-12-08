using System;
using UnityEngine;

[Serializable]
public class QuestReferenceDataType
{
    [Tooltip("The guid of this quest.")]
    public string guid;
    [Tooltip("The state this quest is in.")]
    public int state;
    [Tooltip("Whether this quest can be progressed on the same day (false) or not (true)")]
    public bool dayLimited;
    [Tooltip("Only relevant when dayLimited is true. Day count when this quest has been last progressed.")]
    public int dayLimitedLastTick;

    public QuestReferenceDataType(string guid, int state, bool dayLimtied, int dayLimitedLastTick)
    {
        this.guid = guid;
        this.state = state;
        this.dayLimited = dayLimtied;
        this.dayLimitedLastTick = dayLimitedLastTick;
    }
}
