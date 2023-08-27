using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.HealthSystemCM
{

    public class HealthBarUI : MonoBehaviour
    {

        [SerializeField] private GameObject getHealthSystemGameObject;

        [Tooltip("Image to show the Health Bar, should be set as Fill, the script modifies fillAmount")]
        [SerializeField] private Image healthImage;

        [Tooltip("Image to show the Shield Bar, should be set as Fill, the script modifies fillAmount")]
        [SerializeField] private Image shieldImage;

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
            healthImage.fillAmount = healthSystem.GetHealthNormalized();
        }

        private void UpdateShieldBar()
        {
            shieldImage.fillAmount = healthSystem.GetShieldNormalized();
        }

        private void OnDestroy()
        {
            healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
            // Assuming you added this event in your HealthSystem class
            healthSystem.OnShieldChanged -= HealthSystem_OnShieldChanged;
        }
    }
}
