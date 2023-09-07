using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    public int slots = 5; // Number of inventory slots
    public NetworkList<int> items = new NetworkList<int>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);


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

        //activeSlot.OnValueChanged += OnActiveSlotChanged;

        DeactivateAllSlots();
    }

    [ServerRpc]
    public void AddItemServerRpc(int item, ServerRpcParams rpcParams = default)
    {
        if (items.Count < slots)
        {
            items.Add(item);
            UpdateUI();
            AddItemClientRpc(item);
        }
    }

    [ClientRpc]
    private void AddItemClientRpc(int item, ClientRpcParams rpcParams = default)
    {
        // Synchronized across all clients
        // The item addition to 'items' list on the server has automatically synchronized it
        // You just need to update the UI or any other logic that's necessary on clients
        UpdateUI();
    }

    public int GetFreeSlotIndex() // returns the index 
    {
        if (items.Count < slots)
        {
            return items.Count;
        }

        return -1; // Inventory full
    }

    public void AddToSlots(int index,GameObject gameObject)
    {
        Instantiate(gameObject, itemSlots[index].transform);
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
        if (ItemManager.Instance.GetItemById(items[index]) is WeaponItem)
        {
            return PlayerState.Shooting;
        }
        else if (ItemManager.Instance.GetItemById(items[index]) is HealingItem)
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
