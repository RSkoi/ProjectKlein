using System;
using UnityEngine;

[Serializable]
public class LocalisationDataType
{
    [TextArea]
    public string text;
    public string name;
    public LocalisationNamePosEnum namePos;
    public bool isNarrator;
    public float sizeIncrease;
}
