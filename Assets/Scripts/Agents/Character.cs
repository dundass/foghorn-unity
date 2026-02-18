using UnityEngine;

public class Character : MonoBehaviour, IAgent {

    public bool hostile { get; set; }

    public Agenda[] agendas;
    public Island[] heritage;
    public IAgent[] trusts;
    public Inventory inventory;
    
}
