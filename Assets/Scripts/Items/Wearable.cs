using UnityEngine;

[CreateAssetMenu(fileName = "New Wearable", menuName = "Items/Wearable")]
public class Wearable : Item
{
    public WearableSlot slot;

    public override void Use()
    {
        base.Use();
        WearableManager.Instance.Equip(this);
        RemoveFromInventory();
    }
}

public enum WearableSlot
{
    Head,
    Chest,
    Legs,
    Feet,
    Weapon,
    Shield,
    Neck,
    Finger
}