using MoreMountains.Tools;
using Unity.Netcode;
using UnityEngine;


public class HealthBarUI : NetworkBehaviour
{

    [SerializeField] private GameObject getHealthSystemGameObject;

    [SerializeField] public MMProgressBar healthBar;
    [SerializeField] public MMProgressBar shieldBar;

    private HealthSystem healthSystem;

    [SerializeField] private NetworkObject networkObject;

    public override void OnNetworkSpawn()
    {
        if (networkObject.IsLocalPlayer)
        {
            if (HealthSystem.TryGetHealthSystem(getHealthSystemGameObject, out HealthSystem healthSystem))
            {
                SetHealthSystem(healthSystem);
            }
        }
        else
        {
            // Disable this UI for non-local players
            this.transform.parent.gameObject.SetActive(false);
        }

        base.OnNetworkSpawn();
    }

    public void SetHealthSystem(HealthSystem healthSystem)
    {
        if (this.healthSystem != null)
        {
            this.healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
            // Assuming you added this event in your HealthSystem class
            this.healthSystem.OnShieldChanged -= HealthSystem_OnShieldChanged;
        }
        this.healthSystem = healthSystem;

        UpdateHealthBar();
        UpdateShieldBar();

        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        // Assuming you added this event in your HealthSystem class
        healthSystem.OnShieldChanged += HealthSystem_OnShieldChanged;
    }

    private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        UpdateHealthBar();
    }

    private void HealthSystem_OnShieldChanged(object sender, System.EventArgs e)
    {
        UpdateShieldBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.UpdateBar01(healthSystem.GetHealthNormalized());
    }

    private void UpdateShieldBar()
    {
        shieldBar.UpdateBar01(healthSystem.GetShieldNormalized());
    }

    public override void OnDestroy()
    {
        healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
        // Assuming you added this event in your HealthSystem class
        healthSystem.OnShieldChanged -= HealthSystem_OnShieldChanged;

        base.OnDestroy();
    }
}

