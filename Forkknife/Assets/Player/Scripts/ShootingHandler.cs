using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ShootingHandler : NetworkBehaviour, IActionHandler
{
    public GameObject ActiveSlot { get; set; }
    private Weapon currentWeapon;

    [SerializeField] private IKControl ikControl; // Drag your IKControl component here in the inspector
    [SerializeField] private float recoilStrength = 0.5f;

    public UnityEvent onShoot;

    public void ConfigureWeapon()
    {
        currentWeapon = ActiveSlot.GetComponentInChildren<Weapon>();
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NetworkLog.LogInfoServer($"1. Pressed mouse button - {NetworkObjectId}");
            FireBulletServerRpc(CodeMonkey.Utils.UtilsClass.GetMouseWorldPosition());
        }
    }

    [ServerRpc]
    private void FireBulletServerRpc(Vector3 mousePos, ServerRpcParams rpcParams = default)
    {
        NetworkLog.LogInfoServer($"2. Is shoot delay finished and ready to shoot? - {NetworkObjectId}");
        bool shootPassed = currentWeapon.Shoot();
        if (shootPassed)
        {
            NetworkLog.LogInfoServer($"3. Shooting ready - {NetworkObjectId}");

            currentWeapon.FireBullet(mousePos);

            HandleShootingEffectsClientRpc(mousePos);
            onShoot?.Invoke();
        }
        else
        {
            NetworkLog.LogInfoServer($"3. Shooting not ready, returning! - {NetworkObjectId}");
        }
    }

    [ClientRpc]
    private void HandleShootingEffectsClientRpc(Vector3 mousePos, ClientRpcParams rpcParams = default)
    {
        currentWeapon.PlayShootParticle();
        HandleRecoil(mousePos);
    }

    private void HandleRecoil(Vector3 mousePos)
    {
        Vector3 shootingDirection = (mousePos - transform.position).normalized;

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
