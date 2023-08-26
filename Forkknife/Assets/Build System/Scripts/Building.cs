using System;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour, IDamageable, IRepairable
{
    [SerializeField] private float maxHealth = 100f;
    public StructureType StructureType;
    public float CurrentHealth {  get; private set; }
    
    private bool isBuilding = false;

    [SerializeField] private float buildRate = 10f;

    public event Action<Building> OnBuildingDestroyed;

    public UnityEvent onBuilding;


    private void Awake()
    {
        CurrentHealth = 0.01f;
        StartBuilding();
    }

    private void Update()
    {
        if (isBuilding && CurrentHealth < maxHealth)
        {
            Repair(buildRate * Time.deltaTime);
        }

        if(CurrentHealth <= 0)
        {
            OnBuildingDestroyed?.Invoke(this);
        }
    }

    public void StartBuilding()
    {
        isBuilding = true;
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        UpdateBuildingVisuals();
    }

    public void Repair(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > maxHealth)
        {
            CurrentHealth = maxHealth;
            isBuilding = false;
        }
        UpdateBuildingVisuals();
    }

    private void UpdateBuildingVisuals()
    {

    }
}
