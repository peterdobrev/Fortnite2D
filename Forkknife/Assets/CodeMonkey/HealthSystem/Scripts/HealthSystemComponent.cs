using UnityEngine;

namespace CodeMonkey.HealthSystemCM
{

    /// <summary>
    /// Adds a HealthSystem to a Game Object
    /// </summary>
    public class HealthSystemComponent : MonoBehaviour, IGetHealthSystem
    {

        [Tooltip("Maximum Health amount")]
        [SerializeField] private float healthAmountMax = 100f;

        [Tooltip("Maximum Shield amount")]
        [SerializeField] private float shieldAmountMax = 100f;

        [Tooltip("Starting Health amount, leave at 0 to start at full health.")]
        [SerializeField] private float startingHealthAmount;

        [Tooltip("Starting Shield amount, leave at 0 to start with no shield.")]
        [SerializeField] private float startingShieldAmount;

        private HealthSystem healthSystem;


        private void Awake()
        {
            // Create Health System
            healthSystem = new HealthSystem(healthAmountMax);

            if (startingHealthAmount != 0)
            {
                healthSystem.SetHealth(startingHealthAmount);
            }
            if (startingShieldAmount != 0)
            {
                healthSystem.ApplyShield(startingShieldAmount);
            }
        }

        /// <summary>
        /// Get the Health System created by this Component
        /// </summary>
        public HealthSystem GetHealthSystem()
        {
            return healthSystem;
        }
    }

}