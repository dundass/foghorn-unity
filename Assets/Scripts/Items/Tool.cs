using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Items/Tool")]
public class Tool : Item {

    public override void Use()
    {
        base.Use();
        // apply effects from inherited list
        foreach (IEffect effect in GetEffects())
        {
            //effect.Apply(gameObject);
        }
    }
}
