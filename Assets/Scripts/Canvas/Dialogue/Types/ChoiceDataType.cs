using System;

[Serializable]
public class ChoiceDataType
{
    public string text;

    public bool setsFlag;
    [DrawIf("setsFlag", true)]
    public string flagId;
    [DrawIf("setsFlag", true)]
    public int flagValue = -1;

    public bool requiresFlag;
    [DrawIf("requiresFlag", true)]
    public string requiresFlagId;
    [DrawIf("requiresFlag", true)]
    public int requiresFlagValue = -1;

    public bool hasNestedLoc;
    [DrawIf("hasNestedLoc", true)]
    public LocalisationData nestedLoc;
}
