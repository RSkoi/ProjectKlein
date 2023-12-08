using System;
using UnityEngine;

[Serializable]
public class ParticleSystemDataType
{
    public string name;
    public GameObject prefab;
    public bool destroyFlag = false;

    public ParticleSystemDataType(ParticleSystemDataType prefillData)
    {
        if (prefillData != null)
        {
            name = prefillData.name;
            prefab = prefillData.prefab;
        }
    }
}
