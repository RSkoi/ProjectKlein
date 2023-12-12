using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NextNodeListType
{
    public string sceneName;
    [SerializeReference]
    public List<ScriptedCondition> sceneTravelConditions = new();
}
