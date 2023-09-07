using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : NetworkBehaviour
{
    public Item item;

    public UnityEvent onPickup;

    private Inventory playerInventory;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && IsOwner)
        {
            playerInventory = collision.GetComponent<Inventory>();
            var inventorySlotIndex = playerInventory.GetFreeSlotIndex();
            if (inventorySlotIndex > -1)
            {
                playerInventory.AddItemServerRpc(item.itemId);
                // We're telling the server that we want to pick up the item
                HandlePickupServerRpc(collision.GetComponent<NetworkObject>().NetworkObjectId);
            }
        }
    }

    [ServerRpc]
    private void HandlePickupServerRpc(ulong playerNetworkObjectId, ServerRpcParams rpcParams = default)
    {
        // Server confirms that the item can be picked up and then tells the corresponding client to visually represent this item in the UI
        AddToSlotsClientRpc(playerNetworkObjectId);
        GetComponent<NetworkObject>().Despawn(true);
    }

    [ClientRpc]
    private void AddToSlotsClientRpc(ulong playerNetworkObjectId)
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkObjectId];
        var playerInv = playerObject.GetComponent<Inventory>();
        int index = playerInv.GetFreeSlotIndex()-1;
        if (index > -1)
        {
            playerInv.AddToSlots(index, item.equippableObject);
        }
    }


}
