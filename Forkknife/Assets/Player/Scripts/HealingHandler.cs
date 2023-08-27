using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HEALING HERE ALSO WORKS FOR SHIELDS
/// </summary>
public class HealingHandler : MonoBehaviour, IActionHandler
{
    public GameObject activeSlot;
    private Healing currentHealing;

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartHealing();
        }
    }

    public void ConfigureHealing()
    {
        currentHealing = activeSlot.GetComponentInChildren<Healing>();
    }

    private void StartHealing()
    {
        StopAllCoroutines();
        StartCoroutine(HealingWithDelay(currentHealing.healingItem.timeToUse));
    }

    private IEnumerator HealingWithDelay(float time)
    {
        yield return new WaitForSeconds(time);
        Heal();  
    }

    private void Heal()
    {
        switch(currentHealing.healingItem.healingType)
        {
            case HealingType.Health:
                break;
            case HealingType.Shield: 
                break;
        }
    }


}
