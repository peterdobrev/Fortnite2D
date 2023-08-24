using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory playerInventory;
    public Transform itemsParent;
    public GameObject inventoryUI;

    public Image[] itemImages;

    private void Start()
    {
        itemImages = itemsParent.GetComponentsInChildren<Image>();
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < itemImages.Length; i++)
        {
            if (i < playerInventory.items.Count)
            {
                itemImages[i].sprite = playerInventory.items[i].item.icon;
                itemImages[i].enabled = true;
            }
            else
            {
                itemImages[i].enabled = false;
            }
        }
    }
}