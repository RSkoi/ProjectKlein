public class FlagCondition : ScriptedCondition
{
    private FlagManager _flagManager;

    public string requiredFlagId;
    public int requiredFlagValue;

    public FlagCondition()
    {
        conditionName = "FlagCondition";
        writesConditionRefusedText = false;
        conditionRefusedTextIsSetByScript = false;
    }

    public new void InitDynamic()
    {
        _flagManager = PlayerSingleton.Instance.flagManager;
    }

    public new bool CheckCondition()
    {
        InitDynamic();

        bool result = _flagManager.ContainsFlag(requiredFlagId);
        if (invertCondition)
            result = !result;

        return result;
    }
}
