using System;
using UnityEngine;

[Serializable]
public class SaveFileDataType
{
    [Tooltip("The version of the game this savefile was made in.")]
    public string gameVersion;
    [Tooltip("The custom name for this savefile. Default is the timestamp.")]
    public string saveName;
    [Tooltip("The timestamp of the save.")]
    public string timestamp;
    [Tooltip("The scene name of the save.")]
    public string sceneName;
    [Tooltip("The SO states of the save")]
    public SaveStatesDataType saveStates;
    [Tooltip("The items data.")]
    public ItemCrossSceneDataType itemsData;
    [Tooltip("The journal data.")]
    public JournalCrossSceneDataType journalData;
    [Tooltip("The visited nodes data.")]
    public MapNodeVisitedDictionary visitedNodes;
    [Tooltip("The flag data.")]
    public FlagDataType[] flagData;
    [Tooltip("The day/night cycle data")]
    public DNCycleDataType dnCycleData;
}
