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
    public AnimationCurve moveSpeedCurve = new();
    public bool animated;
    public string animatedState;
    public Vector3 size = new(1, 1, 1);
    public Vector2 posOffset = new(0, 0);
    public bool flipped = false;
    public EntityPrefabTypeEnum prefabType = EntityPrefabTypeEnum.UnboundMiddle;

    public EntityDataType(EntityDataType prefillData)
    {
        if (prefillData != null)
        {
            entityName = prefillData.entityName;
            relativeMovePos = prefillData.relativeMovePos;
            fadedOut = prefillData.fadedOut;
            entityImage = prefillData.entityImage;
            moveDuration = prefillData.moveDuration;
            moveSpeedCurve = new(prefillData.moveSpeedCurve.keys);
            animated = prefillData.animated;
            animatedState = prefillData.animatedState;
            size = new(prefillData.size.x, prefillData.size.y, prefillData.size.z);
            posOffset = new(prefillData.posOffset.x, prefillData.posOffset.y);
            flipped = prefillData.flipped;
            prefabType = prefillData.prefabType;
        }
    }
}
