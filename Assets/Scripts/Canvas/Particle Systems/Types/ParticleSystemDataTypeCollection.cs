using System;
using System.Collections.Generic;

[Serializable]
public class ParticleSystemDataTypeCollection
{
    public List<ParticleSystemDataType> particleSystems;

    public ParticleSystemDataTypeCollection(ParticleSystemDataTypeCollection prefillData)
    {
        if (prefillData != null)
        {
            particleSystems = new();
            foreach (ParticleSystemDataType data in prefillData.particleSystems) 
                particleSystems.Add(new(data));
        }
    }
}
