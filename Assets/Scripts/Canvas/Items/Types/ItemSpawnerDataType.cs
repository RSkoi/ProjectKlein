using System;
using UnityEngine;

[Serializable]
public class ItemSpawnerDataType
{
    [Tooltip("The item to be spawned.")]
    public ItemData item;
    [Tooltip("The max quantity of item the player can receive. Bottom limit is always 1.")]
    public int quantityRangeMax = 1;
    [Tooltip("How many items this spawner can give at maximum. -1 is unlimited.")]
    public int spawnerCapacity = -1;
}
