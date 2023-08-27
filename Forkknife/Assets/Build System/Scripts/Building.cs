using System;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour, IDamageable, IRepairable
{
    [SerializeField] private float maxHealth = 100f;
    public StructureType StructureType;
    public float CurrentHealth {  get; private set; }
    public Animator animator;
    
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
        animator.SetBool("isBuilding", true);

        switch (StructureType)
        {
            default:
            case StructureType.Wall:
                animator.SetInteger("BuildingType", 1);
                break;
            case StructureType.Floor:
                animator.SetInteger("BuildingType", 2);
                break;
            case StructureType.Ramp:
            case StructureType.ReversedRamp:
                animator.SetInteger("BuildingType", 3);
                break;
        }
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
            animator.SetBool("isBuilding", false);
        }
        UpdateBuildingVisuals();
    }

    private void UpdateBuildingVisuals()
    {

    }
}
