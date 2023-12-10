using UnityEngine;

public class PackageCondition : ScriptedCondition
{
    private NodeManager _nodeManager;

    private string _nextPackageName;
    public string packageName;

    public PackageCondition()
    {
        conditionName = "PackageCondition";
        writesConditionRefusedText = true;
        conditionRefusedTextIsSetByScript = true;
    }

    public new void InitDynamic()
    {
        if (conditionRefusedTextIsSetByScript || conditionRefusedText.Equals(DEFAULT_CONDITION_REFUSED_TEXT))
            conditionRefusedText = $"Cannot travel outside of {packageName}";

        _nodeManager = PlayerSingleton.Instance.nodeManager;
        _nextPackageName = _nodeManager._travelNextPackageAndSceneName;
    }

    public new bool CheckCondition()
    {
        InitDynamic();

        bool result = _nextPackageName.StartsWith(packageName);
        if (invertCondition)
            result = !result;

        return result;
    }
}
