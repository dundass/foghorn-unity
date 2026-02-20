using UnityEngine;

public abstract class IEffect : ScriptableObject
{
    public abstract void Apply(GameObject target);
}
