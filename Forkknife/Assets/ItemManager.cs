using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public List<Item> allItems; 
    private Dictionary<int, Item> itemLookup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // optional

        itemLookup = new Dictionary<int, Item>();
        foreach (var item in allItems)
        {
            itemLookup[item.itemId] = item;
        }
    }

    public Item GetItemById(int id)
    {
        return itemLookup.TryGetValue(id, out var item) ? item : null;
    }
}
