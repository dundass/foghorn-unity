using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddItem : MonoBehaviour, IEffect
{
    [SerializeField] List<IItem> items = new List<IItem>(); // todo - amounts?

    public void Apply(GameObject target)
    {
        Inventory inventory = target.GetComponent<Inventory>();
        if(inventory != null)
        {
            foreach(IItem item in items)
            {
                inventory.Add(item);
            }
        }
    }
}
