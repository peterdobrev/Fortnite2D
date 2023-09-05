using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public GameObject equipabbleObject;

    public UnityEvent onPickup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int inventorySlotIndex = collision.GetComponent<Inventory>().Add(item);
            if (inventorySlotIndex > -1)
            {
                collision.GetComponent<Inventory>().AddToGameObjectItemSlots(inventorySlotIndex, equipabbleObject);
                onPickup.Invoke();
                Destroy(gameObject); // Destroy item from the world
            }
        }
    }
}
