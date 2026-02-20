using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AddItem Effect", menuName = "Effects/AddItem")]
public class AddItem : IEffect
{
    [SerializeField] private List<Item> items = new List<Item>(); // todo - amounts?

    public override void Apply(GameObject target)
    {
        Inventory inventory = target.GetComponent<Inventory>();
        if(inventory != null)
        {
            foreach(Item item in items)
            {
                inventory.Add(item);
            }
        }
    }
}
