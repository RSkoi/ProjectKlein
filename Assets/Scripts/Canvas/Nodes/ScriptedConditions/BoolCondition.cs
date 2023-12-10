public class BoolCondition : ScriptedCondition
{
    public bool setBool;

    public BoolCondition()
    {
        conditionName = "BoolCondition";
        writesConditionRefusedText = true;
        conditionRefusedTextIsSetByScript = true;
    }

    public new void InitDynamic()
    {
        if (conditionRefusedTextIsSetByScript || conditionRefusedText.Equals(DEFAULT_CONDITION_REFUSED_TEXT))
            conditionRefusedText = $"BoolCondition {conditionName}: {setBool}";
    }

    public new bool CheckCondition()
    {
        InitDynamic();

        bool result = setBool;
        if (invertCondition)
            result = !result;

        return result;
    }
}
