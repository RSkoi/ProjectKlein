using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TooltipLookupData")]
public class TooltipLookupData : ScriptableObject
{
    [Tooltip("The lookup table for tooltups. Associates a tooltip string with an ID.")]
    public TooltipReferenceDictionary tooltips;
}
