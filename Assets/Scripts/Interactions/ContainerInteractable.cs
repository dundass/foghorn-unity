using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class ContainerInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] List<IItem> items = new List<IItem>();

    private Inventory _inventory;

    private void Start()
    {
        _inventory = GetComponent<Inventory>();
        
        foreach(IItem item in items)
        {
            _inventory.Add(item);
        }
    }

    public void Interact(GameObject target = null)
    {
        // open container and player inventory UI
    }
}
