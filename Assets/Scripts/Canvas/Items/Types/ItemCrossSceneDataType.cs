using System;

[Serializable]
public class ItemCrossSceneDataType
{
    public ItemReferenceDataType[] items;

    public ItemCrossSceneDataType(ItemReferenceDataType[] items)
    {
        this.items = items;
    }
}
