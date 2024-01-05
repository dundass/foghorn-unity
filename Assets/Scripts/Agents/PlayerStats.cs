using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public Inventory inventory;

    private void Awake()
    {
        GameManager.Instance.PlayerStats = this;
    }

    private void Start()
    {
        WearableManager.Instance.onWearablesChanged += OnWearablesChanged;
    }

    private void OnWearablesChanged(Wearable newItem, Wearable oldItem)
    {
        if(newItem != null)
        {
            //armour.AddModifier(newItem.armourModifier);
            //damage.AddModifier(newItem.damageModifier);
        }

        if (oldItem != null)
        {
            //armour.RemoveModifier(oldItem.armourModifier);
            //damage.RemoveModifier(oldItem.damageModifier);
        }
    }
}
