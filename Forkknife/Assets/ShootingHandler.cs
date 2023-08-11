using UnityEngine;
using UnityEngine.Events;

public class ShootingHandler : MonoBehaviour, IActionHandler
{
    public GameObject slot1;
    private IWeapon currentWeapon;

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
    }
}
