using Unity.Netcode;
using UnityEngine;

public class Weapon : NetworkBehaviour, IWeapon, IGetItem
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootingPoint;

    public WeaponItem weaponItem;

    private float nextFireTime = 0f;

    public bool Shoot()
    {
        if (Time.time > nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + 1 / weaponItem.fireRate;
            return true;
        }

        return false;
    }

    private void FireBullet()
    {
        PlayShootParticle();
        if(transform.position.y < 0f )
        {
            Time.timeScale = 0;
        }

        GameObject bulletInstance = InstantiateNetworkedBullet();

        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();


        if (bulletScript)
        {
            Vector2 shootingDirection = GetShootingDirection();
            bulletScript.SetDirection(shootingDirection);
        }
    }

    private GameObject InstantiateNetworkedBullet()
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
        NetworkObject bulletNetworkObject = bulletInstance.GetComponent<NetworkObject>();
        bulletNetworkObject.Spawn();
        return bulletInstance;
    }

    private Vector2 GetShootingDirection()
    {
        Vector2 direction = (CodeMonkey.Utils.UtilsClass.GetMouseWorldPosition() - transform.position).normalized;
        return direction;
    }

    private void PlayShootParticle()
    {
        var particle = ParticleManager.Instance.GetShootParticle();
        particle.transform.position = shootingPoint.position;
        particle.Play();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(shootingPoint.position, 0.1f);
    }

    public Item GetItem()
    {
        return weaponItem;
    }
}
