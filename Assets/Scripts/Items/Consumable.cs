using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
public class Consumable : Item {

    public List<IEffect> effects;

    public override void Use() {
        base.Use();
        // apply effects
        // remove from inventory
    }
}
