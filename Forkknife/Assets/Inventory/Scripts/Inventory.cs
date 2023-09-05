using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    public int slots = 5; // Number of inventory slots
    public List<ItemStack> items = new List<ItemStack>();

    [SerializeField] private GameObject pickaxeSlot;
    [SerializeField] private GameObject[] itemSlots;

    private NetworkVariable<int> activeSlot = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("UI Reference")]
    public InventoryUI inventoryUI; // Drag and drop your InventoryUI GameObject here in the inspector

    private void Start()
    {
        if (IsOwner)
        {
            activeSlot.Value = 0;
        }

        activeSlot.OnValueChanged += OnActiveSlotChanged;

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
        var gameObjectItem = Instantiate(gameobjectItem, itemSlots[index].transform);
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

    // This method is called whenever the active slot changes
    private void OnActiveSlotChanged(int oldValue, int newValue)
    {
        DeactivateAllSlots();

        if (newValue >= 0 && newValue < itemSlots.Length)
        {
            itemSlots[newValue].SetActive(true);
        }
    }

    public void SetActiveSlot(int slotIndex)
    {
        if (IsOwner && slotIndex >= 0 && slotIndex < items.Count)
        {
            activeSlot.Value = slotIndex;
        }
    }

    public GameObject GetActiveSlot()
    {
        return itemSlots[activeSlot.Value];
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
