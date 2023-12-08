using System;
using System.Collections.Generic;

[Serializable]
public class EntityHistoryDataType
{
    public List<EntityDataType> entities;

    public EntityHistoryDataType(EntityHistoryDataType prefillData)
    {
        if (prefillData != null)
        {
            entities = new();
            if (prefillData.entities != null)
                foreach (EntityDataType entity in prefillData.entities)
                    entities.Add(new(entity));
        }
    }
}
