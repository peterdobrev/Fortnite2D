using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Settings")]
    public Inventory playerInventory;

    public Image[] itemImages; // images of the items in the inventory 

    private void Start()
    {
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < itemImages.Length; i++)
        {
            if (i < playerInventory.items.Count)
            {
                SetItemImage(itemImages[i], playerInventory.items[i].item.icon);
            }
            else
            {
                DisableItemImage(itemImages[i]);
            }
        }
    }

    private void SetItemImage(Image image, Sprite icon)
    {
        image.sprite = icon;
        image.enabled = true;
    }

    private void DisableItemImage(Image image)
    {
        image.enabled = false;
    }

    private void SetActiveItemBackground(int index, Color color)
    {
        itemImages[index].transform.parent.GetComponent<Image>().color = color;
    }

    private void ResetAllBackgrounds()
    {
        for (int i = 0; i < itemImages.Length; i++)
        {
            itemImages[i].transform.parent.GetComponent<Image>().color = Color.white;
        }
    }
}
