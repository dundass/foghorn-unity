using UnityEngine;

public class Tool : MonoBehaviour, IItem {

    new public string name { get; set; }
    public float value { get; set; }
    public Sprite icon { get; set; }

    void Init() {

    }

    // use() - should all items have a use method ? or do i need to create Usable, Wearable, Consumable etc interfaces ?
}
