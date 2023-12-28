using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour, IItem, IConsumable {

    new public string name { get; set; }
    public float value { get; set; }
    public Sprite icon { get; set; }

    public void consume() {
        // apply effect
        // destroy self
    }
}
