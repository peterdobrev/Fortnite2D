using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ShootingHandler : NetworkBehaviour, IActionHandler
{
    public GameObject ActiveSlot { get; set; }
    private IWeapon currentWeapon;

    [SerializeField] private IKControl ikControl; // Drag your IKControl component here in the inspector
    [SerializeField] private float recoilStrength = 0.5f;

    public UnityEvent onShoot;

    public void ConfigureWeapon()
    {
        currentWeapon = ActiveSlot.GetComponentInChildren<IWeapon>();
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireBulletServerRpc();
        }
    }

    [ServerRpc]
    private void FireBulletServerRpc(ServerRpcParams rpcParams = default)
    {
        FireBullet();
        FireBulletClientRpc();
    }

    [ClientRpc]
    private void FireBulletClientRpc(ClientRpcParams rpcParams = default)
    {
        FireBullet();
    }

    private void FireBullet()
    {
        bool shootPassed = currentWeapon.Shoot();
        if(shootPassed)
        {
            onShoot?.Invoke();
            HandleRecoil();
        }
    }

    private void HandleRecoil()
    {
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
