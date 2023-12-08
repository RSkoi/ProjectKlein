using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ParticleSystemData")]
public class ParticleSystemData : ScriptableObject
{
    [Tooltip("List of particle system slides. Each slide contains a variable amount of particle systems.")]
    public List<ParticleSystemDataTypeCollection> particleHistory = new();
    [Tooltip("Dialogue indexes the particle systems should be displayed at." +
        "Indexes of this list correspond to indexes of the slides above.")]
    public List<int> indexes = new();
    [Tooltip("State the particle system list is in. Corresponds to index of particle system list.")]
    public int state = 0;
}
