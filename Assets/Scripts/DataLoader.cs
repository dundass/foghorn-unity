using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataLoader : MonoBehaviour {

    // https://stackoverflow.com/questions/716399/how-do-you-get-a-variables-name-as-it-was-physically-typed-in-its-declaration
    // the above link should help when storing animal behaviours etc (c# class/enum member/function names) as string

    public ItemTypes itemTypes;

    public GameObject foodPrefab;
    public GameObject itemPrefab;
    public GameObject characterPrefab;

    void Start() {
        string filePath = Application.dataPath + "/items.json";
        if(File.Exists(filePath)) {
            string itemsAsJson = File.ReadAllText(filePath);
            //Debug.Log(itemsAsJson);
            itemTypes = ItemTypes.CreateFromJSON(itemsAsJson);
            //Debug.Log(itemTypes.food.Length);     // itemTypes.food has length but each element is null ...
            /*GameObject emptyObject = new GameObject();
            foreach(Food food in itemTypes.food) {
                GameObject newFood = (GameObject)Instantiate(emptyObject);
                newFood.transform.parent = parentObjectTransform;   // do i actually need to make gameobjects yet ??
            }*/
        } else {
            Debug.Log("no data file found for items !");
        }
    }
}

[System.Serializable]
public class ItemTypes {
    public Consumable[] food;
    public Tool[] tools;

    public static ItemTypes CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<ItemTypes>(jsonString);
    }
}