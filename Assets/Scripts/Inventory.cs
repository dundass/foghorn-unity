using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour { 

    public List<IItem> items = new List<IItem>();

    // public int space = varies depending on whose inventory it is

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback; // to hook into inventory UI updates

    public void Add(IItem item) {
        items.Add(item);
        checkDelegates();
    }

    public void Remove(IItem item) {
        items.Remove(item);
        checkDelegates();
    }

    private void checkDelegates() {
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

}
