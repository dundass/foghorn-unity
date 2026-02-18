using UnityEngine;

[CreateAssetMenu(fileName = "New Item Fragment", menuName = "Items/Item Fragment")]
public class ItemFragment : Item
{
    // Reference to the complete item this fragment belongs to
    public Item completeItem;

    public override string displayName
    {
        get => completeItem != null ? "Bit of old " + completeItem.displayName : "Unknown Fragment";
        set => _displayName = value;
    }

    public override void Use()
    {
        base.Use();
        // TODO: Find matching fragment in inventory and combine
        // If found, remove both fragments and add the completeItem
        RemoveFromInventory();
    }
}
