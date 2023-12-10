using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    [Tooltip("The name of the item.")]
    public string itemName;
    [TextArea]
    [Tooltip("The description of the item.")]
    public string itemDesc;
    [Tooltip("The UI sprite of the item.")]
    public Sprite itemSprite;
    [Tooltip("The rarity classification of this item.")]
    public ItemRarityEnum rarity = ItemRarityEnum.COMMON;

    public override string ToString()
    {
        return $"<color=#{(int)rarity:X6}>{itemName} [{rarity}]</color>";
    }
}
