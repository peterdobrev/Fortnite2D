using Unity.Netcode;

[System.Serializable]
public class ItemStack
{
    public Item item;
    public int amount;

    public ItemStack(Item newItem, int newAmount)
    {
        item = newItem;
        amount = newAmount;
    }
}
