using System;
using UnityEngine;

[Serializable]
public class NodeTextType
{
    [TextArea]
    public string text;
    public bool requiresFlag;
    [DrawIf("requiresFlag", true)]
    public string flagId;
    [DrawIf("requiresFlag", true)]
    public int flagValue;
    public string viewCharAnim;
}
