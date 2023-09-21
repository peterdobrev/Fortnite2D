using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Weapon : NetworkBehaviour, IWeapon, IGetItem
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootingPoint;

    public WeaponItem weaponItem;

    public float shootingRange = 100f; // Maximum distance the bullet can travel
    public LayerMask shootableLayer;  // Layer to check for shootable objects

    private float nextFireTime = 0f;

    public bool Shoot()
    {
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + 1 / weaponItem.fireRate;
            return true;
        }

        return false;
    }

    public bool FireBullet(Vector3 mousePos)
    {
        Vector2 shootingDirection = GetShootingDirection(mousePos);

        RaycastHit2D hit = Physics2D.Raycast(shootingPoint.position, shootingDirection, shootingRange, shootableLayer);

        if (hit.collider != null)
        {
            float distanceToTarget = Vector2.Distance(shootingPoint.position, hit.point);
            float timeToReachTarget = distanceToTarget / bulletPrefab.GetComponent<Bullet>().speed; // Assuming bulletSpeed is the speed of the bullet.

            SpawnVisualBulletClientRpc(mousePos, timeToReachTarget);

            // The ray hit something
            var idamageable = hit.collider.GetComponent<IDamageable>();
            if (idamageable != null)
            {
                var networkObjectDamageableId = hit.collider.GetComponent<NetworkObject>().NetworkObjectId;

                StartCoroutine(DelayedDamage(idamageable, weaponItem.damage, timeToReachTarget));
                DelayedDamageClientRpc(networkObjectDamageableId, weaponItem.damage, timeToReachTarget);
                return true;
            }
        }
        else
        {
            SpawnVisualBulletClientRpc(mousePos, 1f);
        }

        return false;
    }

    [ClientRpc]
    private void SpawnVisualBulletClientRpc(Vector3 mousePos, float autoDestroyBulletTimer)
    {
        Debug.Log("Spawning visual bullet");
        Vector2 shootingDir = GetShootingDirection(mousePos);
        var bullet = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity).GetComponent<Bullet>();
        bullet.SetDirection(shootingDir);
        bullet.SetTimeAlive(autoDestroyBulletTimer);
    }

    [ClientRpc]
    private void DelayedDamageClientRpc(ulong targetNetworkObjectId, int damageAmount, float delay)
    {
        if (IsServer || IsHost) return;

        NetworkObject targetNetworkObject = NetworkManager.SpawnManager.SpawnedObjects[targetNetworkObjectId];

        if (targetNetworkObject)
        {
            IDamageable target = targetNetworkObject.gameObject.GetComponent<IDamageable>();
            if (target != null)
            {
                StartCoroutine(DelayedDamage(target, damageAmount, delay));
            }
        }

    }

    private IEnumerator DelayedDamage(IDamageable target, int damageAmount, float delay)
    {
        yield return new WaitForSeconds(delay);
        target.TakeDamage(damageAmount);
    }

    private Vector2 GetShootingDirection(Vector3 mousePos)
    {
        Vector2 direction = (mousePos - transform.position).normalized;
        return direction;
    }

    public void PlayShootParticle()
    {
        var particle = ParticleManager.Instance.GetShootParticle();
        particle.transform.position = shootingPoint.position;
        particle.Play();
    }

    public Item GetItem()
    {
        return weaponItem;
    }
}
