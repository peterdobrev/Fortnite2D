using UnityEngine;

public class Weapon : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private Transform shootingPoint;
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
        PlayShootParticle();
        if(transform.position.y < 0f )
        {
            Time.timeScale = 0;
        }

        Vector3 spawnPosition = shootingPoint.position;
        GameObject bulletInstance = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

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
}
