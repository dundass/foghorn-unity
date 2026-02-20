using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
public class Consumable : Item {

    public override void Use() {
        base.Use();
        // apply effects
        foreach (IEffect effect in GetEffects())
        {
            //effect.Apply(gameObject);
        }
        RemoveFromInventory();
    }
}
