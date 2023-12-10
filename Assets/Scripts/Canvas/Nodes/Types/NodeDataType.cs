using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeDataType
{
    [TextArea]
    public string text;
    public float speed;
    public string title;
    // indexes 0-4 == N-E-S-W
    public List<string> nextNodeSceneNames;
    public string packageName;
    [SerializeReference]
    public List<ScriptedCondition> travelConditions = new();
    [SerializeReference]
    public List<ScriptedEffect> travelEffects = new();
}
