using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EntityHistoryData")]
public class EntityHistoryData : ScriptableObject
{
    [Tooltip("List of entity data types. Each slide contains a variable amount of entities.")]
    public List<EntityHistoryDataType> history = new();
    [Tooltip("Dialogue indexes the entites should be displayed at." +
        "Indexes of this list correspond to indexes of the slides above.")]
    public List<int> indexes = new();
    [Tooltip("State the entity list is in. Corresponds to index of entity list.")]
    public int state = 0;
}
