using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HEALING HERE ALSO WORKS FOR SHIELDS
/// </summary>
public class HealingHandler : MonoBehaviour, IActionHandler
{
    private Healing currentHealing;

    [SerializeField] private HealthSystemComponent healthSystem;
    public GameObject ActiveSlot { get; set; }


    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartHealing();
        }
    }

    public void ConfigureHealing()
    {
        currentHealing = ActiveSlot.GetComponentInChildren<Healing>();
    }

    private void StartHealing()
    {
        StopAllCoroutines();
        StartCoroutine(HealingWithDelay(currentHealing.healingItem.timeToUse));
    }

    private IEnumerator HealingWithDelay(float time)
    {
        Debug.Log("Timer started!");
        yield return new WaitForSeconds(time);
        Heal();  
    }

    private void Heal()
    {
        switch(currentHealing.healingItem.healingType)
        {
            case HealingType.Health:
                healthSystem.GetHealthSystem().Heal(currentHealing.healingItem.healthRecovered);
                break;
            case HealingType.Shield:
                healthSystem.GetHealthSystem().ApplyShield(currentHealing.healingItem.healthRecovered);
                break;
        }
        Debug.Log("Healed!");
    }


}
