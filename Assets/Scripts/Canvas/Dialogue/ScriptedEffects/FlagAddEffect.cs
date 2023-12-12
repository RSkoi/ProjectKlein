using System;
using UnityEngine;

[Serializable]
public class FlagAddEffect : ScriptedEffect
{
    private FlagManager _flagManager;
    private DialogueController _dialogueController;
    private NodeManager _nodeManager;

    public string flagId;
    public int flagValue;

    public FlagAddEffect()
    {
        effectName = "FlagAddEffect";
        writesEffectText = false;
        writesEffectTextToNode = false;
        effectTextColor = Color.yellow;
        flagValue = -1;
    }

    public new void InitDynamic()
    {
        effectText = effectTextIsSetByScript || effectText.Equals(DEFAULT_EFFECT_TEXT)
            ? $"Added flag: {flagId}, value {flagValue}" : effectText;

        _flagManager = PlayerSingleton.Instance.flagManager;
        if (!isNodeEffect)
            _dialogueController = PlayerSingleton.Instance.dialogueController;
        else
            _nodeManager = PlayerSingleton.Instance.nodeManager;
    }

    public new void InvokeEffect()
    {
        InitDynamic();

        if (!CheckConditions().conditionsPassed)
            return;

        if (flagValue == -1)
            _flagManager.AddFlag(flagId);
        else
            _flagManager.AddFlag(flagId, flagValue);

        if (writesEffectText && !isNodeEffect)
            _dialogueController.ShowStringCombined(effectText, "", LocalisationNamePosEnum.Middle, effectTextSpeed, false, 0);
        else if (writesEffectTextToNode && isNodeEffect)
            _nodeManager.AddToDesc(effectText);
    }
}