using UnityEngine;
using UnityEngine.Events;

public class ShootingHandler : MonoBehaviour, IActionHandler
{
    public GameObject slot1;
    private IWeapon currentWeapon;

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


        Vector3 shootingDirection = (CodeMonkey.Utils.UtilsClass.GetMouseWorldPositionCinemachine() - transform.position).normalized;

        // Calculate the recoil direction by getting a vector 90 degrees upwards relative to shooting direction
        Vector3 recoilDirection = Vector3.Cross(shootingDirection, Vector3.forward).normalized;

        // Check if the player is flipped (looking to the left). If so, reverse the recoil direction.
        if (transform.localScale.x < 0)
        {
            recoilDirection *= -1;
        }

        ikControl.ApplyRecoil(-recoilDirection * recoilStrength);
    }


}
