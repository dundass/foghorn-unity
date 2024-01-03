using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour, IAgent {

    public bool hostile { get; set; }

    public Agenda[] agendas;
    public Island[] heritage;
    public IAgent[] trusts;
    public Inventory inventory;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
