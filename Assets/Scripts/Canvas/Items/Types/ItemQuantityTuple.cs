using System;
using UnityEngine;

[Serializable]
public class ItemQuantityTuple
{
    [Tooltip("The item data.")]
    public ItemData item;
    [Tooltip("The quantity.")]
    public int quantity = 1;

    public ItemQuantityTuple(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}
