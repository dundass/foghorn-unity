using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearableManager : Singleton<WearableManager>
{
    public delegate void OnWearablesChanged(Wearable newItem, Wearable oldItem);
    public OnWearablesChanged onWearablesChanged;

    private Wearable[] _currentlyWorn;
    private Inventory _playerInventory;

    private void Start()
    {
        _playerInventory = GameManager.Instance.PlayerStats.inventory;

        int numSlots = System.Enum.GetNames(typeof(WearableSlot)).Length;
        _currentlyWorn = new Wearable[numSlots];
    }

    public void Equip(Wearable newItem)
    {
        int slotIndex = (int)newItem.slot;

        Wearable oldItem = null;

        if(_currentlyWorn[slotIndex] != null)
        {
            oldItem = _currentlyWorn[slotIndex];
            _playerInventory.Add(oldItem);
        }

        onWearablesChanged?.Invoke(newItem, oldItem);

        _currentlyWorn[slotIndex] = newItem;
    }

    public void Unequip(int slotIndex)
    {
        if(_currentlyWorn[slotIndex] != null)
        {
            Wearable oldItem = _currentlyWorn[slotIndex];
            _playerInventory.Add(oldItem);

            _currentlyWorn[slotIndex] = null;

            onWearablesChanged?.Invoke(null, oldItem);
        }
    }
}
