using UnityEngine;

public class InventoryUI : MonoBehaviour {

    public Transform itemsParent;
    public GameObject gameManager;
    public Sprite testItemSprite;

    Inventory inventory;

    InventorySlot[] slots;  // List ?

    // Use this for initialization
    void Start() {
        inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
        inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        DataLoader data = gameManager.GetComponent<DataLoader>();

        IItem testItem = new Food();
        testItem.name = "Haggis";
        testItem.value = 5;
        testItem.icon = testItemSprite;
        inventory.Add(testItem);
        //Food dataTestItem = Instantiate(data.itemTypes.food[0]);
        //inventory.Add(dataTestItem)
    }

    // Update is called once per frame
    void Update() {
        // poll inventory keyboard input in order to toggle here
    }

    public void toggleInventory() {
        itemsParent.gameObject.SetActive(!itemsParent.gameObject.activeSelf);
    }

    void UpdateUI() {
        for(int i = 0; i < slots.Length; i++) {
            if(i < inventory.items.Count) {
                slots[i].AddItem(inventory.items[i]);
            }
            else {
                slots[i].ClearSlot();
            }
        }
    }
}
