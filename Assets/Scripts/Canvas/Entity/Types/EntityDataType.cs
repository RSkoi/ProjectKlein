using System;
using UnityEngine;

[Serializable]
public class EntityDataType
{
    public string entityName;
    public EntityPositionEnum relativeMovePos;
    public bool fadedOut;
    public Sprite entityImage;
    public float moveDuration;
}
