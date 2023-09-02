using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int slots = 5; // Number of inventory slots
    public List<ItemStack> items = new List<ItemStack>();

    [SerializeField] private GameObject pickaxeSlot;
    [SerializeField] private GameObject[] itemSlots;

    private int activeSlot = 0;

    [Header("UI Reference")]
    public InventoryUI inventoryUI; // Drag and drop your InventoryUI GameObject here in the inspector

    private void Start()
    {
        DeactivateAllSlots();
    }

    public int Add(Item item) // returns the index 
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item && items[i].amount < item.maxStackSize)
            {
                items[i].amount++;
                UpdateUI();
                return i;
            }
        }

        if (items.Count < slots)
        {
            items.Add(new ItemStack(item, 1));
            UpdateUI();
            return items.Count-1;
        }

        return -1; // Inventory full
    }

    public void AddToGameObjectItemSlots(int index, GameObject gameobjectItem)
    {
        Instantiate(gameobjectItem, itemSlots[index].transform);
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

    public void SwapItems(int firstIndex, int secondIndex)
    {
        if (firstIndex < 0 || firstIndex >= items.Count || secondIndex < 0 || secondIndex >= items.Count)
            return; // Invalid indices

        ItemStack temp = items[firstIndex];
        items[firstIndex] = items[secondIndex];
        items[secondIndex] = temp;

        UpdateUI();
    }


    public void SetActiveSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < items.Count)
        {
            activeSlot = slotIndex;
            DeactivateAllSlots();
            itemSlots[activeSlot].SetActive(true);
            UpdateUI();
        }
    }

    public GameObject GetActiveSlot()
    {
        return itemSlots[activeSlot];
    }

    public PlayerState DeterminePlayerStateFromItemType(int index)
    {
        if (items[index].item is WeaponItem)
        {
            return PlayerState.Shooting;
        }
        else if (items[index].item is HealingItem)
        {
            return PlayerState.Healing;
        }
        else
        {
            Debug.LogWarning($"COULDN'T DETERMINE PLAYER STATE FROM ITEM TYPE ERROR! INDEX: {index}, ITEM: {items[index]}");
            return PlayerState.Building;

        }

    }


    public void DeactivateAllSlots()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].SetActive(false);
        }
    }

    private void UpdateUI()
    {
        if (inventoryUI != null)
        {
            inventoryUI.UpdateUI();
        }
    }
}
