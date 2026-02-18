using UnityEngine;

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
