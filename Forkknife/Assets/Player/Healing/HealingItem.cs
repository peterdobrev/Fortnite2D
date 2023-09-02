using UnityEngine;

[CreateAssetMenu(fileName = "NewHealingItem", menuName = "Inventory/Healing")]
public class HealingItem : Item
{
    public int healthRecovered = 10;
    public float timeToUse = 1f;
    public HealingType healingType = HealingType.Health;

    public override void Use()
    {
        base.Use();
        Debug.Log($"Recovered {healthRecovered} points.");
    }
}