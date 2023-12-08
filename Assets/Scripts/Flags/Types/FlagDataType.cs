using System;
using UnityEngine;

[Serializable]
public class FlagDataType
{
    public string id;
    [SerializeField]
    private int _value = -1;
    public int value
    {
        get { return this._value; }
        set
        {
            if (updateBehaviour == null)
                this._value = value;
            else
                if (updateBehaviour.Invoke(this.id, this._value, value))
                    this._value = value;
        }
    }
    // <id, prevValue, newValue, shouldUpdate>
    // shouldUpdate is return value
    private Func<string, int, int, bool> updateBehaviour;
    [SerializeField]
    public string _behaviorName;

    public FlagDataType(string id, int value = 0, Func<string, int, int, bool> updateBehaviour = null)
    {
        this.id = id;
        this._value = value;
        if (updateBehaviour != null)
            RegisterBehaviour(updateBehaviour);
    }

    public void RegisterBehaviour(Func<string, int, int, bool> updateBehaviour = null)
    {
        this.updateBehaviour = updateBehaviour;
        this._behaviorName = updateBehaviour.Method.Name;
    }

    public void RegisterBehaviour(string behaviourName)
    {
        if (behaviourName.Equals(""))
            return;

        bool behaviourFound = FlagUpdateBehaviours.BehaviourLookupData.TryGetValue(behaviourName, out var behaviour);
        if (behaviourFound)
            RegisterBehaviour(behaviour);
    }
}
