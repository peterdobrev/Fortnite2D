using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour, IHealing, IGetItem
{
    public HealingItem healingItem;

    public Item GetItem()
    {
        return healingItem;
    }

    public void Heal()
    {
        Debug.Log("Not implemented behaviour!");
    }
}
