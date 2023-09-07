using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int itemId; // Unique ID for each item
    public string itemName = "New Item";
    public Sprite icon = null; // Icon to be displayed in the inventory
    public GameObject equippableObject = null;
    public GameObject droppableObject = null;

    public virtual void Use()
    {
        // What happens when this item is used
        Debug.Log($"Using item: {itemName}");
    }
}
