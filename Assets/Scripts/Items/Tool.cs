using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Items/Tool")]
public class Tool : Item {

    public IEffect effect; // maybe ?

    public override void Use()
    {
        base.Use();
    }
}
