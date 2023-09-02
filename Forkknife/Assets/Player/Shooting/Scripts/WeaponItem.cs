using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponItem", menuName = "Inventory/Weapon")]
public class WeaponItem : Item
{
    public float fireRate = 1f;
    public int bulletCapacity = 30;
    public int damage = 10;

    public override void Use()
    {
        base.Use();
    }
}