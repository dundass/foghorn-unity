using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddItem : MonoBehaviour, IEffect
{
    [SerializeField] List<Item> items = new List<Item>(); // todo - amounts?

    public void Apply(GameObject target)
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
