using UnityEngine;

public class Item : ScriptableObject
{
    public string displayName;
    public int value;
    public Sprite icon;

    public virtual void Use()
    {
        // nothing ?
    }
}
