using UnityEngine;

public class Weapon : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 1f;
    private float nextFireTime = 0f;

    public void Shoot()
    {
        if (Time.time > nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + 1 / fireRate;
        }
    }

    private void FireBullet()
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
        
        if (bulletScript)
        {
            Vector2 shootingDirection = GetShootingDirection();
            bulletScript.SetDirection(shootingDirection);
        }
    }

    private Vector2 GetShootingDirection()
    {
        Vector2 direction = (CodeMonkey.Utils.UtilsClass.GetMouseWorldPosition() - transform.position).normalized;
        return direction;
    }
}
