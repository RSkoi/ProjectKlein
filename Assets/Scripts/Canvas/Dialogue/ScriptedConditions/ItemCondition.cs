public class ItemCondition : ScriptedCondition
{
    private InventoryManager _inventoryManager;

    public ItemData requiredItem;
    public int requiredQuantity;

    public ItemCondition()
    {
        conditionName = "ItemCondition";
        writesConditionRefusedText = true;
        conditionRefusedTextIsSetByScript = true;
    }

    public new void InitDynamic()
    {
        if (conditionRefusedTextIsSetByScript || conditionRefusedText.Equals(DEFAULT_CONDITION_REFUSED_TEXT))
            conditionRefusedText = $"Required {requiredItem} x {requiredQuantity} to travel";

        _inventoryManager = PlayerSingleton.Instance.inventoryManager;
    }

    public new bool CheckCondition()
    {
        InitDynamic();

        bool result = _inventoryManager.HasQuantityOfItem(requiredItem, requiredQuantity);
        if (invertCondition)
            result = !result;

        return result;
    }
}
