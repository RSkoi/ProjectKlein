using System;

[Serializable]
public class ItemAddEffect : ScriptedEffect
{
    private InventoryManager _inventoryManager;
    private DialogueController _dialogueController;
    private NodeManager _nodeManager;

    public ItemData itemToAdd;
    public int quantityToAdd;

    public ItemAddEffect()
    {
        effectName = "ItemAddEffect";
        writesEffectText = true;
        //effectTextColor = quantityToAdd < 0 ? Color.red : Color.cyan;
        quantityToAdd = 0;
    }

    public new void InitDynamic()
    {
        bool add = quantityToAdd >= 0;
        effectText = effectTextIsSetByScript || effectText.Equals(DEFAULT_EFFECT_TEXT)
            ? $"{(add ? "Added" : "Removed")} Item: {itemToAdd} x {(add ? quantityToAdd : quantityToAdd * -1)}" : effectText;
        
        _inventoryManager = PlayerSingleton.Instance.inventoryManager;
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

        if (quantityToAdd < 0)
            _inventoryManager.RemoveFromInventory(itemToAdd, quantityToAdd * -1);
        else
            _inventoryManager.AddToInventory(itemToAdd, quantityToAdd);

        if (writesEffectText && !isNodeEffect)
            _dialogueController.ShowStringCombined(effectText, "", LocalisationNamePosEnum.Middle, effectTextSpeed, false, 0);
        else if (writesEffectTextToNode && isNodeEffect)
            _nodeManager.AddToDesc(effectText);
    }
}