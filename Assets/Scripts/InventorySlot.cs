using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    public GameObject inspector;
    public Image icon;

    Item item;

    public void AddItem(Item newItem) {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot() {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    void OnMouseOver() { // onhover item inspector - none of this works cuz unity is shit
        Debug.Log("mouseover");
        if (item == null) {
            inspector.SetActive(false);
            return;
        }
        inspector.SetActive(true);
        Text text = inspector.GetComponentInChildren<Text>();
        text.text = item.name;
    }
    

}
