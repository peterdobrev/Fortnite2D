using UnityEngine;

[CreateAssetMenu(fileName = "NewHealingItem", menuName = "Inventory/Healing")]
public class HealingItem : Item
{
    public int healthRecovered = 10;
    public float timeToUse = 1f;

    public override void Use()
    {
        base.Use();
        // Add logic to recover player's health by `healthRecovered` amount
        Debug.Log($"Recovered {healthRecovered} health.");
    }
}