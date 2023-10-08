using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ShootingHandler : NetworkBehaviour, IActionHandler
{
    public GameObject ActiveSlot { get; set; }
    private Weapon currentWeapon;
    private bool canShoot;

    [SerializeField] private IKControl ikControl; // Drag your IKControl component here in the inspector
    [SerializeField] private float recoilStrength = 0.5f;

    public UnityEvent onShoot;
    public UnityEvent onShotgunShoot;
    public UnityEvent onARShoot;
    public UnityEvent onPistolShoot;
    public UnityEvent onSniperShoot;

    private void Awake()
    {
        onShoot.AddListener(InvokeSpecificWeaponShootEvent);
    }

    public void ConfigureWeapon()
    {
        canShoot = false;
        currentWeapon = ActiveSlot.GetComponentInChildren<Weapon>();
        StartCoroutine(EnableShootingAfterDelay(0.5f)); 
    }

    private IEnumerator EnableShootingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!canShoot) return;

            FireBulletServerRpc(CodeMonkey.Utils.UtilsClass.GetMouseWorldPosition());
        }
    }

    [ServerRpc]
    private void FireBulletServerRpc(Vector3 mousePos, ServerRpcParams rpcParams = default)
    {
        bool shootPassed = currentWeapon.Shoot();

        if (shootPassed)
        {
            currentWeapon.FireBullet(mousePos);
            HandleShootingEffectsClientRpc(mousePos);
            onShoot?.Invoke();
        }
        else
        {
            NetworkLog.LogInfoServer($"Shooting not ready, returning! - {NetworkObjectId}");
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

    private WeaponType GetWeaponType()
    {
        return currentWeapon.weaponItem.weaponType;
    }

    private void InvokeSpecificWeaponShootEvent()
    {
        var weapon = GetWeaponType();
        switch (weapon)
        {
            case WeaponType.AR:
                onARShoot.Invoke();
                break;
            case WeaponType.Sniper:
                onSniperShoot.Invoke();
                break;
            case WeaponType.Pistol:
                onPistolShoot.Invoke();
                break;
            case WeaponType.Shotgun:
                onShotgunShoot.Invoke();
                break;
            default: break;
            
        }
    }

}
