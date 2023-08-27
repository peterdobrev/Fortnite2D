using UnityEngine;

[CreateAssetMenu(fileName = "NewShieldItem", menuName = "Inventory/Shield")]
public class ShieldItem : Item
{
    public int shieldRecovered = 10;
    public float timeToUse = 1f;

    public override void Use()
    {
        base.Use();
        // Add logic to recover player's health by `healthRecovered` amount
        Debug.Log($"Recovered {shieldRecovered} health.");
    }
}