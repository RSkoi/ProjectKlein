using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ScriptedEffect
{
    public static Dictionary<ScriptedEffectEnum, Type> scriptedEffectTypes = new()
    {
        { ScriptedEffectEnum.ItemAddEffect, typeof(ItemAddEffect) },
        { ScriptedEffectEnum.TrackQuestEffect, typeof(TrackQuestEffect) },
    };

    public enum ScriptedEffectEnum
    {
        ItemAddEffect,
        TrackQuestEffect,
    }

    public static Type GetTypeOfDerivedEffect(ScriptedEffect effect)
    {
        foreach (Type type in scriptedEffectTypes.Values)
            if (effect.GetType() == type)
                return type;
        return typeof(ScriptedEffect);
    }

    public static readonly string DEFAULT_EFFECT_NAME = "DefaultEffectName";
    public static readonly bool DEFAULT_IS_NODE_EFFECT = false;
    public static readonly bool DEFAULT_WRITES_EFFECT_TEXT = false;
    public static readonly bool DEFAULT_WRITES_EFFECT_TEXT_TO_NODE = false;
    public static readonly bool DEFAULT_EFFECT_TEXT_SET_BY_SCRIPT = true;
    public static readonly string DEFAULT_EFFECT_TEXT = "Debug: DefaultEffectName";
    public static readonly Color DEFAULT_EFFECT_TEXT_COLOR = Color.white;
    public static readonly float DEFAULT_EFFECT_TEXT_SPEED = 0.01f;

    public string effectName = DEFAULT_EFFECT_NAME;
    public bool isNodeEffect = DEFAULT_IS_NODE_EFFECT;
    [SerializeReference]
    public List<ScriptedCondition> conditions = new();
    public bool writesEffectText = DEFAULT_WRITES_EFFECT_TEXT;
    [DrawIf("isNodeEffect", true)]
    public bool writesEffectTextToNode = DEFAULT_WRITES_EFFECT_TEXT_TO_NODE;
    public bool effectTextIsSetByScript = DEFAULT_EFFECT_TEXT_SET_BY_SCRIPT;
    [DrawIf("writesEffectText", true)]
    public string effectText = DEFAULT_EFFECT_TEXT;
    [DrawIf("writesEffectText", true)]
    public Color effectTextColor = DEFAULT_EFFECT_TEXT_COLOR;
    [DrawIf("writesEffectText", true)]
    public float effectTextSpeed = DEFAULT_EFFECT_TEXT_SPEED;

    public void InitDynamic() { Debug.LogError("Scripted Effect does not hide default method InitDynamic()"); }
    public void InvokeEffect() { Debug.LogError("Scripted Effect does not hide default method InvokeEffect()"); }

    public string GetEffectText()
    {
        if (writesEffectText || writesEffectTextToNode)
            return $"<color=#{effectTextColor.ToHexString()}>{effectText}</color>";
        return "";
    }

    public (List<string> refusedConditionsText, bool conditionsPassed) CheckConditions()
    {
        List<string> refusedConditionsText = ScriptedCondition.CheckConditions(conditions);
        return (refusedConditionsText, refusedConditionsText.Count == 0);
    }

    public static void InvokeEffects(List<ScriptedEffect> effects)
    {
        if (effects.Count > 0)
            foreach (ScriptedEffect effect in effects)
            {
                Type castType = GetTypeOfDerivedEffect(effect);
                dynamic castEffect = Convert.ChangeType(effect, castType);
                castEffect.InvokeEffect();
            }
    }
}
