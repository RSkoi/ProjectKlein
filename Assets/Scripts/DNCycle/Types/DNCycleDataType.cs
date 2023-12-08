using System;
using UnityEngine;

[Serializable]
public class DNCycleDataType
{
    public bool isDay = true;
    public bool newDay = false;

    public DNCycleDataType(DNCycleDataType data)
    {
        if (data != null)
        {
            isDay = data.isDay;
            newDay = data.newDay;
        }
    }
}
