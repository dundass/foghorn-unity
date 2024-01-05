using System.Collections;
using UnityEngine;

[System.Serializable]
public class BaseEffect : IEffect
{
    // was gonna try inheriting the other effects from this and making them serializable too
    // but unity still doesn't let me drag them onto an Item SO :(

    public void Apply(GameObject target)
    {
        
    }
}