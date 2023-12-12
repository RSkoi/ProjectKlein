using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeQuestStateDataType
{
    /* TODO: this whole type is a workaround as SerializableDictionary does not support lists
     * as values (not displayed in editor), remove this when support is added */ 

    [Tooltip("The quests attached to a node.")]
    public List<QuestData> quests;
    [Tooltip("Quest states. Parallel list to above quests list.")]
    public List<int> states;

    public NodeQuestStateDataType(List<QuestData> quests, List<int> states)
    {
        this.quests = quests;
        this.states = states;
    }
}
