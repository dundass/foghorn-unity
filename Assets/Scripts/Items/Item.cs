using UnityEngine;
using System.Collections.Generic;

public class Item : ScriptableObject
{
    [SerializeField]
    protected string _displayName;
    public virtual string displayName
    {
        get => _displayName;
        set => _displayName = value;
    }

    public int value;
    public Sprite icon;
    [SerializeField]
    private List<IEffect> effects = new();
    
    public List<IEffect> GetEffects() => effects;

    public virtual void Use()
    {
        // nothing ?
    }

    public void RemoveFromInventory()
    {
        // get player inventory
        // remove item
    }
}
