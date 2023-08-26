using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    public UnityEvent onPickup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            bool wasPickedUp = collision.GetComponent<Inventory>().Add(item);
            if (wasPickedUp)
            {
                onPickup.Invoke();
                //Destroy(gameObject); // Destroy item from the world
            }
        }
    }
}
