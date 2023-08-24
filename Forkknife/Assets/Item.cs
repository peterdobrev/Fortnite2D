using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon = null; // Icon to be displayed in the inventory
    public int maxStackSize = 1; // Default to 1 for items that can't be stacked

    public virtual void Use()
    {
        // What happens when this item is used
        Debug.Log($"Using item: {itemName}");
    }
}
