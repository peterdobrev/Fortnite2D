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
            nextFireTime = Time.time + 1 / weaponItem.fireRate;
            return true;
        }

        return false;
    }

    public void FireBullet(Vector3 mousePos)
    {
        // this method is already only being handled by the server so maybe it is not needed to be a server rpc
        NetworkLog.LogInfoServer($"4. Sending firing information to the server - {NetworkObjectId}");
        SpawnBulletServerAuth(mousePos);
        
    }

    //[ServerRpc]
    private void SpawnBulletServerAuth(Vector3 mousePos)
    {
        if (!IsHost && !IsServer) {
            NetworkLog.LogInfoServer($"5. IS NOT SERVER OR HOST, RETURNING - {NetworkObjectId}");
            return; }
        NetworkLog.LogInfoServer($"5. Client is host or server - {NetworkObjectId}");
        GameObject bulletInstance = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
        NetworkLog.LogInfoServer($"6. Bullet instantiated - {NetworkObjectId}");
        NetworkObject bulletNetworkObject = bulletInstance.GetComponent<NetworkObject>();
        
        

        bulletNetworkObject.Spawn();
        NetworkLog.LogInfoServer($"8. Bullet spawned on server - {NetworkObjectId}");

        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
        if (bulletScript)
        {
            Vector2 shootingDirection = GetShootingDirection(mousePos);
            bulletScript.SetDirection(shootingDirection);
        }
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
