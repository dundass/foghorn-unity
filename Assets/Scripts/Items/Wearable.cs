using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wearable", menuName = "Items/Wearable")]
public class Wearable : Item
{
    public WearableSlot slot;
    public List<IEffect> effects;

    public override void Use()
    {
        base.Use();
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