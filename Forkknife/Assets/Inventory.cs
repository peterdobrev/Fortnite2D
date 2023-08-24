using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int slots = 5; // Number of inventory slots
    public List<ItemStack> items = new List<ItemStack>();

    public bool Add(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item && items[i].amount < item.maxStackSize)
            {
                items[i].amount++;
                return true;
            }
        }

        if (items.Count < slots)
        {
            items.Add(new ItemStack(item, 1));
            return true;
        }

        return false; // Inventory full
    }

    public void Use(Item item)
    {
        item.Use();
        // Further implementation: Remove item or reduce stack size, etc.
    }

    public void Remove(Item item)
    {
        // Implementation: Remove item or reduce stack size
    }
}
