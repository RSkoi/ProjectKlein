using System;
using System.Collections.Generic;
using UnityEngine;

public class FlagUpdateBehaviours
{
    // key is method name, value is method delegate
    public static Dictionary<string, Func<string, int, int, bool>> BehaviourLookupData = new()
    {
        { "LogFlagChange", LogFlagChange }
    };

    public static bool LogFlagChange(string id, int prev, int now)
    {
        Debug.Log($"flag with id {id} had value {prev} now {now}");
        return true;
    }
}
