using MoreMountains.Tools;
using UnityEngine;


public class HealthBarUI : MonoBehaviour
{

    [SerializeField] private GameObject getHealthSystemGameObject;

    [SerializeField] public MMProgressBar healthBar;
    [SerializeField] public MMProgressBar shieldBar;

    private HealthSystem healthSystem;

    private void Start()
    {
        if (HealthSystem.TryGetHealthSystem(getHealthSystemGameObject, out HealthSystem healthSystem))
        {
            SetHealthSystem(healthSystem);
        }
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

    private void OnDestroy()
    {
        healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
        // Assuming you added this event in your HealthSystem class
        healthSystem.OnShieldChanged -= HealthSystem_OnShieldChanged;
    }
}

