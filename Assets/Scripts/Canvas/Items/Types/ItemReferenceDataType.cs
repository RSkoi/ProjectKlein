using System;
using UnityEngine;

[Serializable]
public class ItemReferenceDataType
{
    [Tooltip("The guid of this quest.")]
    public string guid;
    [Tooltip("The quantity of held item.")]
    public int quantity;

    public ItemReferenceDataType(string guid, int quantity)
    {
        this.guid = guid;
        this.quantity = quantity;
    }
}
