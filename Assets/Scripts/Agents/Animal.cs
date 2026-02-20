using UnityEngine;

public class Animal : MonoBehaviour, IAgent {

    public enum reactions { fight, flight }

    public bool hostile { get; set; }
    // var idlebehaviour    // maybe put these in IAgent ?
    // var attackbehaviour
    // var reaction (= reactions.fight/reactions.flight)
    // var drops[] // how to implement chances ?

}
