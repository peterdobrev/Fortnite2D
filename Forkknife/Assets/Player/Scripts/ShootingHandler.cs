using UnityEngine;
using UnityEngine.Events;

public class ShootingHandler : MonoBehaviour, IActionHandler
{
    public GameObject slot1;
    private IWeapon currentWeapon;

    [SerializeField] private Transform shootParticleSpawnPoint;
    [SerializeField] private IKControl ikControl; // Drag your IKControl component here in the inspector
    [SerializeField] private float recoilStrength = 0.5f;

    public UnityEvent onShoot;

    private void Awake()
    {
        currentWeapon = slot1.GetComponentInChildren<IWeapon>();
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireBullet();
        }
        else if (Input.GetMouseButtonDown(1))
        {
        }
    }

    private void FireBullet()
    {
        currentWeapon?.Shoot();
        onShoot?.Invoke();

        PlayShootParticle();

        // Apply recoil (it should be multiplied by -1, but in my code here this is how it works correctly)
        Vector3 recoilDirection = (CodeMonkey.Utils.UtilsClass.GetMouseWorldPosition() - transform.position).normalized;
        ikControl.ApplyRecoil(recoilDirection * recoilStrength);
    }

    private void PlayShootParticle()
    {
        var particle = ParticleManager.Instance.GetShootParticle();
        particle.transform.position = shootParticleSpawnPoint.position;
        particle.Play();
    }
}
