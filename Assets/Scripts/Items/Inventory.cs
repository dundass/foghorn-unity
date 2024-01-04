using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour { 

    public List<Item> items = new List<Item>();

    // public int space = varies depending on whose inventory it is

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback; // to hook into inventory UI updates

    public void Add(Item item) {
        items.Add(item);
        CheckDelegates();
    }

    public void Remove(Item item) {
        items.Remove(item);
        CheckDelegates();
    }

    private void CheckDelegates() {
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

}
