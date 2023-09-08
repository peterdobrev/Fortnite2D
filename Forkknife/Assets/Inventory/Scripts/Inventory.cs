using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    public int slots = 3; // Number of inventory slots
    public GameObject[] itemSlots;


    [Header("UI Reference")]
    public InventoryUI inventoryUI; // Drag and drop your InventoryUI GameObject here in the inspector

    private int activeSlot = 0;

    private void Start()
    {
        DeactivateAllSlots();
    }

    // Client calls this method to request slot change
    public void SetActiveSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots)
        {
            activeSlot = slotIndex;
        }
    }

    public void CycleBetweenItems(int slotIndex)
    {
        for (int i = 0; i < itemSlots[slotIndex].transform.childCount; i++)
        {
            if(itemSlots[slotIndex].transform.GetChild(i).gameObject.activeSelf == true)
            {
                NetworkLog.LogInfoServer("1a Cycling begin" + $" {NetworkBehaviourId}");
                int nextIndex = (i + 1) % itemSlots[slotIndex].transform.childCount;
                itemSlots[slotIndex].transform.GetChild(i).gameObject.SetActive(false);
                itemSlots[slotIndex].transform.GetChild(nextIndex).gameObject.SetActive(true);
                UpdateUI();
                NetworkLog.LogInfoServer("1b Cycling end " + $" {NetworkBehaviourId}");
                return;
            }
        }
    }

    public int GetActiveSlotIndex()
    {
        return activeSlot;
    }

    public GameObject GetActiveSlot()
    {
        NetworkLog.LogInfoServer($"2 Active slot- ActiveSlot[{activeSlot}] - {itemSlots[activeSlot]}" + $" {NetworkBehaviourId}");
        return itemSlots[activeSlot];
    }

    public void DeactivateAllSlots()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].SetActive(false);
        }
        NetworkLog.LogInfoServer("3 Deactivating all slots" + $" {NetworkBehaviourId}");
    }

    private void UpdateUI()
    {
        if (inventoryUI != null)
        {
            inventoryUI.UpdateUI();
        }
    }
}
