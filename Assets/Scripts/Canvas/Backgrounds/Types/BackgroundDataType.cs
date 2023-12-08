using System;
using UnityEngine;

[Serializable]
public class BackgroundDataType
{
    public Texture texture;
    public bool fadeBlackTransition;
    public bool fadeAlphaTransition;

    public BackgroundDataType(BackgroundDataType prefillData)
    {
        if (prefillData != null)
        {
            texture = prefillData.texture;
            fadeBlackTransition = prefillData.fadeBlackTransition;
            fadeAlphaTransition = prefillData.fadeAlphaTransition;
        }
    }
}
