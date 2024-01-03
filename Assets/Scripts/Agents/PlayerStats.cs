using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private void Start()
    {
        // EquipmentManager.Instance.onEquipmentChanged += OnEquipmentChanged;
    }

    private void OnEquipmentChanged(/* Wearable newItem, Wearable oldItem */)
    {
        /*if(newItem != null)
        {
            armour.AddModifier(newItem.armourModifier);
            damage.AddModifier(newItem.damageModifier);
        }

        if (oldItem != null)
        {
            armour.RemoveModifier(oldItem.armourModifier);
            damage.RemoveModifier(oldItem.damageModifier);
        }*/
    }
}
