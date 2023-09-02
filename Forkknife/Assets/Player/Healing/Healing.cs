using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour, IHealing
{
    public HealingItem healingItem;
    public ItemPickup droppableObject;

    public void Heal()
    {
        Debug.Log("Not implemented behaviour!");
    }
}
