using System;
using UnityEngine;

[Serializable]
public class NodeTextType
{
    [TextArea]
    public string text;
    public bool requiresFlag;
    public string flagId;
    public int flagValue;
    public string viewCharAnim;
}
