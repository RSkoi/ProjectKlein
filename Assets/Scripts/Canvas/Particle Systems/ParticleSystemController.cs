using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    // note: this 'dictionary' will not be editable in Editor even if public, that requires a SerializableDictionary
    private readonly List<(ParticleSystemDataType, GameObject)> _particleSystemInstances = new();

    public GameObject particleSystemContainer;

    public void Spawn(ParticleSystemDataTypeCollection particles)
    {
        // pooling doesn't really work here, unless you want to painstakingly replace one particle system with another
        // just don't instantiate too many and disable instead of destroy
        /*if (particles.particleSystems.Count > particleSystemContainer.transform.childCount)
            Populate(particles.particleSystems.Count - particleSystemContainer.transform.childCount);*/

        foreach (ParticleSystemDataType particle in particles.particleSystems)
        {
            // clones with the same name are not allowed
            foreach ((ParticleSystemDataType psdt, GameObject go) in _particleSystemInstances)
            {
                if (psdt.name.Equals(particle.name))
                {
                    if (particle.destroyFlag)
                    {
                        psdt.name += "_disabled";
                        go.SetActive(false);
                    }
                    return;
                }
                else if (psdt.name.Equals($"{particle.name}_disabled"))
                {
                    if (!particle.destroyFlag)
                    {
                        psdt.name = particle.name;
                        go.SetActive(true);
                    }
                    return;
                }
            }

            Spawn(particle);
        }
    }

    public void Spawn(ParticleSystemDataType particle)
    {
        GameObject go = Instantiate(particle.prefab, particleSystemContainer.transform, false);
        _particleSystemInstances.Add((particle, go));
    }

    public void Despawn(ParticleSystemDataTypeCollection particles)
    {
        foreach (ParticleSystemDataType particle in particles.particleSystems)
            Despawn(particle);
    }

    public void Despawn(ParticleSystemDataType particle)
    {
        foreach ((ParticleSystemDataType psdt, GameObject go) in _particleSystemInstances)
            if (psdt.name.Equals(particle.name))
                go.SetActive(false);
    }

    public void ResetAll()
    {
        foreach ((ParticleSystemDataType psdt, GameObject go) in _particleSystemInstances)
        {
            int index = psdt.name.IndexOf("_disabled");
            if (index != -1)
                psdt.name = psdt.name.Remove(index, "_disabled".Length);
        }
    }

    public void ResetSO(List<ParticleSystemDataTypeCollection> particleCollections)
    {
        foreach (ParticleSystemDataTypeCollection pscol in particleCollections)
            foreach (ParticleSystemDataType psdt in pscol.particleSystems)
            {
                int index = psdt.name.IndexOf("_disabled");
                if (index != -1)
                    psdt.name = psdt.name.Remove(index, "_disabled".Length);
            }
    }
}
