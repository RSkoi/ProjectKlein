using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemLookupData")]
public class ItemLookupData : ScriptableObject
{
    [Tooltip("The lookup table for items. Associates an item SO with a GUID.")]
    public ItemReferenceDictionary lookupTable;
}
