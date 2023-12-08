using System;
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public FlagData flagData;

    [Tooltip("Collection of branching scene loads as Dictionary(CombinedFlag, SceneName). " +
        "Due to limitations CombinedFlag should be formatted as \"FlagID#FlagValue\" with the # char as separator. " +
        "FlagValue of -1 will be treated as a wildcard; the key for the SceneName is then effectively NOT (FlagID, FlagValue) but FlagID alone. " +
        "Flags with wildcards will be deleted on next scene load if SceneDirector.nextSceneDecidedByFlagManager is set to true.")]
    public FlagSceneDictionary branchingScenes = new();

    public string GetNextSceneByFlag(string fallback)
    {
        string sceneName = fallback;
        foreach (string flag in branchingScenes.Keys)
        {
            (string flagId, int flagValue) = SplitBranchingFlag(flag);
            if (flagValue == -1
                ? ContainsFlag(flagId)
                : ContainsFlag(flagId, flagValue))
            {
                sceneName = branchingScenes[$"{flagId}#{flagValue}"];
                // increase flag value if not wildcard
                if (flagValue != -1)
                    AddFlag(flagId, flagValue + 1);
                else
                    DeleteFlag(flagId);
            }
        }

        return sceneName;
    }

    private (string flagId, int flagValue) SplitBranchingFlag(string flag)
    {
        string[] splitFlag = flag.Split('#');
        return splitFlag.Length == 2 ? (splitFlag[0], int.Parse(splitFlag[1])) : (splitFlag[0], -1);
    }

    public bool ContainsFlag(string id)
    {
        foreach (FlagDataType flag in flagData.flags)
            if (flag.id.Equals(id))
                return true;

        return false;
    }

    public bool ContainsFlag(string id, int value)
    {
        foreach (FlagDataType flag in flagData.flags)
            if (flag.id.Equals(id) && flag.value == value)
                return true;

        return false;
    }

    public int GetFlagIndex(string id)
    {
        for (int i = 0; i < flagData.flags.Count; i++)
            if (flagData.flags[i].id.Equals(id))
                return i;

        return -1;
    }

    public void ClearFlags()
    {
        flagData.flags.Clear();
    }

    public void AddFlag(string id, int value = 0, Func<string, int, int, bool> updateBehaviour = null)
    {
        int i = GetFlagIndex(id);
        if (i > -1)
            flagData.flags[i].value = value;
        else
            flagData.flags.Add(new FlagDataType(id, value, updateBehaviour));
    }

    public bool IncrementFlag(string id)
    {
        int i = GetFlagIndex(id);
        if (i == -1)
            return false;

        flagData.flags[i].value++;
        return true;
    }

    public bool DeleteFlag(string id)
    {
        // there's gotta be a better way to do this
        FlagDataType flagToDelete = null;
        foreach (FlagDataType flag in flagData.flags)
        {
            if (flag.id.Equals(id))
            {
                flagToDelete = flag;
                break;
            }
        }

        if (flagToDelete != null)
        {
            flagData.flags.Remove(flagToDelete);
            return true;
        }
        
        return false;
    }

    public void SetFlags(FlagDataType[] flags)
    {
        // this could be done by clearing and iterative AddFlag instead
        // I trust the garbage collector to not fuck this up
        flagData.flags = new List<FlagDataType>(RegisterFlagBehaviours(flags));
    }

    public FlagDataType[] RegisterFlagBehaviours(FlagDataType[] flags)
    {
        // re-register flag behaviours, as those are not serialized/saved in save file
        foreach (FlagDataType flag in flags)
            flag.RegisterBehaviour(flag._behaviorName);
        return flags;
    }

    public FlagDataType[] PrepareFlagEntriesForSave()
    {
        return flagData.flags.ToArray();
    }

    public FlagDataType GetFlag(int index)
    {
        return flagData.flags[index];
    }
}
