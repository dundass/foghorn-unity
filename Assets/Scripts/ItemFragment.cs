using UnityEngine;

public class ItemFragment : MonoBehaviour, IItem {

    public new string name { get => "Bit of old " + name; set => name = value; }
    public float value { get; set; }
    public Sprite icon { get; set; }

    void use() {
        // find another item of type
        // if exists, destroy both and add non-fragmented item
    }

}
