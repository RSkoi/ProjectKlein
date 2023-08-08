using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    [Tooltip("Entity container for the entity instances.")]
    public GameObject entityContainer;
    [Tooltip("The template prefab for new entities.")]
    public Object entityPrefab;
    [Tooltip("List of instantiated entity instance hierarchies.")]
    public List<Entity> entityInstances = new();

    public void RenderEntitySlide(List<EntityDataType> entities)
    {
        (List<EntityDataType> toInitialize, List<(Entity, EntityDataType)> toUpdate, List<Entity> toDestroy)
            = MapEntities(entities);

        IntializeEntities(toInitialize);
        UpdateEntities(toUpdate);
        DestroyEntities(toDestroy);
    }

    private void IntializeEntities(List<EntityDataType> entities)
    {
        foreach (EntityDataType e in entities)
        {
            GameObject newEntity = Instantiate(entityPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

            // renaming as first step to avoid reordering into a hierarchy with a GO of the same name
            newEntity.name = e.entityName;
            newEntity.transform.SetParent(entityContainer.transform);

            // need to reposition and resize newEntity again, as position and scale are inherited from parent
            // TODO: set y dynamically
            newEntity.transform.SetLocalPositionAndRotation(new(0, -42f, 0), Quaternion.identity);
            newEntity.transform.localScale = Vector3.one;

            Entity newEntityComponent = newEntity.GetComponent<Entity>();

            // set image
            newEntityComponent.SwitchTexture(e.entityImage, e.animated, e.animatedState);

            // set name
            newEntityComponent.entityName = e.entityName;

            // teleport entity to set position in history
            newEntityComponent.Teleport(e.relativeMovePos);

            // set direction
            newEntityComponent.curEntityPos = e.relativeMovePos;

            // set faded
            Color fadeTo = e.fadedOut ? Entity.BACKGROUND_COLOR : new(1, 1, 1);
            newEntityComponent.entityImage.color = new(1, 1, 1, 0);
            newEntityComponent.FadeInstantiate(fadeTo);
            newEntityComponent.fadedOut = e.fadedOut;

            entityInstances.Add(newEntityComponent);
        }
    }

    private void UpdateEntities(List<(Entity, EntityDataType)> entityMap)
    {
        foreach ((Entity e, EntityDataType edt) in entityMap)
        {
            // move
            e.Move(edt.relativeMovePos, edt.moveDuration);

            // put in fore-/background
            if (e.fadedOut && !edt.fadedOut)
                e.PutInForeground();
            else if (!e.fadedOut && edt.fadedOut)
                e.PutInBackground();

            // change sprite
            if (e.entityImage.sprite != edt.entityImage || e.animatedState != edt.animatedState)
                e.SwitchTexture(edt.entityImage, edt.animated, edt.animatedState);

            e.gameObject.name = edt.entityName;
        }
    }

    private void DestroyEntities(List<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            entityInstances.Remove(e);
            e.FadeDestroy();
        }
    }

    private EntityDataType EntityNameInList(string name, List<EntityDataType> entities)
    {
        foreach (EntityDataType e in entities)
            if (e.entityName.Equals(name))
                return e;

        return null;
    }

    // TODO: make more efficient? nested foreach
    private (List<EntityDataType>, List<(Entity, EntityDataType)>, List<Entity>)
        MapEntities(List<EntityDataType> entities)
    {
        List<EntityDataType> toInitialize = new(entities);
        List<(Entity, EntityDataType)> toUpdate = new();
        List<Entity> toDestroy = new();

        foreach (Entity e in entityInstances)
        {
            EntityDataType edt = EntityNameInList(e.entityName, entities);
            if (edt != null)
            {
                toInitialize.Remove(edt);
                toUpdate.Add((e, edt));
            }
            else
                toDestroy.Add(e);
        }

        return (toInitialize, toUpdate, toDestroy);
    }
}
