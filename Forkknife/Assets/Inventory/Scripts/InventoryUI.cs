using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : NetworkBehaviour
{
    [Header("Settings")]
    public Inventory playerInventory;

    public Image[] itemImages; // images of the items in the inventory 

    [SerializeField] private NetworkObject networkObject;

    public override void OnNetworkSpawn()
    {
        // Only initialize if this is the local player
        if (networkObject.IsLocalPlayer)
        {
            InitializeInventory();
        }
        else
        {
            // Disable this UI for non-local players
            this.transform.parent.gameObject.SetActive(false);
        }

        base.OnNetworkSpawn();
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
                SetItemImage(itemImages[i], ItemManager.Instance.GetItemById(playerInventory.items[i]).icon);
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
