public class Organisation {

    Agenda[] agendas { get; set; }
    IAgent[] members { get; set; }
    int age { get; set; }

    public Organisation(Agenda[] agendas, IAgent[] members) {
        this.agendas = agendas;
        this.members = members;
        age = 0;
    }

    public float totalWealth() {
        float tot = 0;
        foreach (IAgent member in members) {
            // if(member has Inventory)
        }
        return tot;
    }

    public int membershipSize() {
        return members.Length;
    }

    public void update() {
        age++;
    }

}
