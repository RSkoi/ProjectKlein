using System;
using System.Collections.Generic;

[Serializable]
public class AudioEffectDataTypeCollection
{
    public AudioEffectDataType[] effects;
    public bool stopAllLoopingEffects = false;

    public AudioEffectDataTypeCollection(AudioEffectDataTypeCollection prefillData)
    {
        if (prefillData != null)
        {
            List<AudioEffectDataType> newEffects = new();
            if (prefillData.effects != null)
                foreach (AudioEffectDataType effect in prefillData.effects)
                    newEffects.Add(new(effect));
            effects = newEffects.ToArray();

            stopAllLoopingEffects = prefillData.stopAllLoopingEffects;
        }
    }
}
