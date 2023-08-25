using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int slots = 5; // Number of inventory slots
    public List<ItemStack> items = new List<ItemStack>();

    [Header("UI Reference")]
    public InventoryUI inventoryUI; // Drag and drop your InventoryUI GameObject here in the inspector

    public bool Add(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item && items[i].amount < item.maxStackSize)
            {
                items[i].amount++;
                UpdateUI();
                return true;
            }
        }

        if (items.Count < slots)
        {
            items.Add(new ItemStack(item, 1));
            UpdateUI();
            return true;
        }

        return false; // Inventory full
    }

    public void Use(Item item)
    {
        item.Use();
        // Further implementation: Remove item or reduce stack size, etc.
        UpdateUI();
    }

    public void Remove(Item item)
    {
        // Implementation: Remove item or reduce stack size
        // After removing, don't forget to call:
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (inventoryUI != null)
        {
            inventoryUI.UpdateUI();
        }
    }
}
