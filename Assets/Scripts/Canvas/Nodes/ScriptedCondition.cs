using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ScriptedCondition
{
    public static Dictionary<ScriptedConditionEnum, Type> travelConditionTypes = new()
    {
        { ScriptedConditionEnum.ItemCondition, typeof(ItemCondition) },
        { ScriptedConditionEnum.FlagCondition, typeof(FlagCondition) },
        { ScriptedConditionEnum.PackageCondition, typeof(PackageCondition) },
        { ScriptedConditionEnum.BoolCondition, typeof(BoolCondition) },
    };

    public enum ScriptedConditionEnum
    {
        ItemCondition,
        FlagCondition,
        PackageCondition,
        BoolCondition,
    }

    public enum ConditionLogicalCombinationEnum
    {
        // force cnf by user
        //AND,
        OR
    }

    public static Type GetTypeOfDerivedTravelCondition(ScriptedCondition condition)
    {
        foreach (Type type in travelConditionTypes.Values)
            if (condition.GetType() == type)
                return type;
        return typeof(ScriptedCondition);
    }

    public static readonly string DEFAULT_CONDITION_NAME = "DefaultConditionName";
    public static readonly bool DEFAULT_INVERT_CONDITION = false;
    public static readonly bool DEFAULT_WRITES_CONDITION_REFUSED_TEXT = true;
    public static readonly bool DEFAULT_CONDITION_REFUSED_TEXT_SET_BY_SCRIPT = false;
    public static readonly Color DEFAULT_CONDITION_REFUSED_TEXT_COLOR = Color.red;
    public static readonly string DEFAULT_CONDITION_REFUSED_TEXT = $"{DEFAULT_CONDITION_NAME}: false";
    public static readonly bool DEFAULT_CONDITION_REQUIRES_FLAG = false;
    public static readonly string DEFAULT_CONDITION_REQUIRED_FLAG_ID = "";
    public static readonly int DEFAULT_CONDITION_REQUIRED_FLAG_VALUE = -1;

    public string conditionName = DEFAULT_CONDITION_NAME;
    public bool invertCondition = DEFAULT_INVERT_CONDITION;
    public bool writesConditionRefusedText = DEFAULT_WRITES_CONDITION_REFUSED_TEXT;
    [DrawIf("writesConditionRefusedText", true)]
    public bool conditionRefusedTextIsSetByScript = DEFAULT_CONDITION_REFUSED_TEXT_SET_BY_SCRIPT;
    [DrawIf("writesConditionRefusedText", true)]
    public string conditionRefusedText = DEFAULT_CONDITION_REFUSED_TEXT;
    [DrawIf("writesConditionRefusedText", true)]
    public Color conditionRefusedTextColor = DEFAULT_CONDITION_REFUSED_TEXT_COLOR;
    public bool conditionRequiresFlag = DEFAULT_CONDITION_REQUIRES_FLAG;
    [DrawIf("conditionRequiresFlag", true)]
    public string conditionRequiredFlagId = DEFAULT_CONDITION_REQUIRED_FLAG_ID;
    [DrawIf("conditionRequiresFlag", true)]
    public int conditionRequiredFlagValue = DEFAULT_CONDITION_REQUIRED_FLAG_VALUE;

    // <ScriptedCondition.conditionName, ConditionLogicalCombinationEnum>
    public TravelConditionDictionary boundConditions = new();

    public void InitDynamic() { Debug.LogError("ScriptedCondition does not hide default method InitDynamic()"); }
    public bool CheckCondition() { Debug.LogError("ScriptedCondition does not hide default method CheckCondition()"); return false; }

    public string GetConditionRefusedText()
    {
        if (writesConditionRefusedText)
            return $"<color=#{conditionRefusedTextColor.ToHexString()}>{conditionRefusedText}</color>";
        return "";
    }

    // returns List<ScriptedCondition.conditionName, result>
    public List<(string, bool)> CheckCombinedConditions(bool value)
    {
        List<(string, bool)> result = new();

        NodeManager _nodeManager = PlayerSingleton.Instance.nodeManager;
        foreach ((string conditionName, ConditionLogicalCombinationEnum logicalOperator) in boundConditions)
        {
            // limit search to first result
            ScriptedCondition condition = _nodeManager.nodeData.node.travelConditions
                .Where(x => x.conditionName.Equals(conditionName))
                .First();
            Type castType = GetTypeOfDerivedTravelCondition(condition);
            dynamic castCondition = Convert.ChangeType(condition, castType);

            // force cnf by user
            /*switch (logicalOperator)
            {
                case ConditionLogicalCombinationEnum.AND:
                    result.Add((conditionName, value && condition.CheckCondition()));
                    break;
                case ConditionLogicalCombinationEnum.OR:
                    result.Add((conditionName, value || castCondition.CheckCondition(true)));
                    break;
            }*/

            result.Add((conditionName, castCondition.CheckCondition()));
        }

        return result;
    }

    public static List<string> CheckConditions(List<ScriptedCondition> conditions)
    {
        List<string> failedConditionsText = new();
        foreach (ScriptedCondition condition in conditions)
        {
            Type castType = GetTypeOfDerivedTravelCondition(condition);
            dynamic castCondition = Convert.ChangeType(condition, castType);
            bool conditionResult = castCondition.CheckCondition();

            List<(string, bool)> boundConditionsResults = null;
            if (condition.boundConditions.Count > 0)
                boundConditionsResults = castCondition.CheckCombinedConditions(conditionResult);

            bool boundCnfResult = conditionResult;
            if (boundConditionsResults != null)
                foreach ((string boundConditionName, bool boundConditionResult) in boundConditionsResults)
                    boundCnfResult = boundCnfResult || boundConditionResult;

            // cnf evaluates to false if one part is false
            if (!boundCnfResult)
                failedConditionsText.Add(castCondition.GetConditionRefusedText());
        }
        return failedConditionsText;
    }
}
