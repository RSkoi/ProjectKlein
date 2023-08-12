using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NodeData")]
public class NodeData : ScriptableObject
{
    [Tooltip("The node data of this scene.")]
    public NodeDataType node;
    [Tooltip("The item spawners of this scene.")]
    public List<ItemSpawnerDataType> itemSpawners;
    [Tooltip("The quest states attached to this node. Key is quest name")]
    public QuestStateDictionary questStates;
}
