using System;
using UnityEngine;

[Serializable]
public class SaveFileDataType
{
    [Tooltip("The scene name of the quicksave.")]
    public string sceneName;
    [Tooltip("The dialogue state of the quicksave.")]
    public int dialogueState;
    [Tooltip("The background state of the quicksave.")]
    public int backgroundState;
    [Tooltip("The entity state of the quicksave.")]
    public int entityState;
    [Tooltip("The background state of the quicksave.")]
    public int bgSongsState;
    [Tooltip("The background state of the quicksave.")]
    public int audioEffectsState;
    [Tooltip("The items data.")]
    public ItemCrossSceneDataType itemsData;
    [Tooltip("The journal data.")]
    public JournalCrossSceneDataType journalData;
}
