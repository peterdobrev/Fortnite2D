using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            bool wasPickedUp = collision.GetComponent<Inventory>().Add(item);
            if (wasPickedUp)
            {
                Destroy(gameObject); // Destroy item from the world
            }
        }
    }
}
