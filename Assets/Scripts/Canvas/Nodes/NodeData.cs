using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NodeData")]
public class NodeData : ScriptableObject
{
    [Tooltip("The node data of this scene.")]
    public NodeDataType node;
    [Tooltip("The item spawners of this scene.")]
    public List<ItemSpawnerDataType> itemSpawners = new();
    [Tooltip("The quest states attached to this node. Key is quest name")]
    public QuestStateDictionary questStates;
    [Tooltip("The quest states attached to this node that should be automatically added to the journal on scene start. " +
        "This should only be used for starting quests, NOT updating tracked quests or their values.")]
    public List<QuestStateDataType> defaultQuestStates = new();
    [Tooltip("List of text prompts to be randomly spit out at exploring this node")]
    public List<NodeTextType> randomTextOnExplore = new();
}
