using CodeMonkey.HealthSystemCM;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[RequireComponent(typeof(BuildingHandler))]
[RequireComponent(typeof(ShootingHandler))]
[RequireComponent(typeof(HealingHandler))]
[RequireComponent(typeof(Inventory))]
public class PlayerActionController2D : NetworkBehaviour, IGetHealthSystem
{
    public PlayerState CurrentState { get; private set; }
    private BuildingHandler buildingHandler;
    private ShootingHandler shootingHandler;
    private HealingHandler healingHandler;
    private Inventory inventory;

    [SerializeField] private GameObject[] slots;
    [SerializeField] private GameObject[] weapons;

    public UnityEvent onWallKeyPressed;
    public UnityEvent onFloorKeyPressed;
    public UnityEvent onRampKeyPressed;
    public UnityEvent onReversedRampKeyPressed;

    public UnityEvent onSlot1KeyPressed;
    public UnityEvent onSlot2KeyPressed;
    public UnityEvent onSlot3KeyPressed;

    public UnityEvent onBuildingMode;
    public UnityEvent onShootingMode;
    public UnityEvent onHealingMode;

    private void Awake()
    {
        CurrentState = PlayerState.Shooting;

        buildingHandler = GetComponent<BuildingHandler>();
        shootingHandler = GetComponent<ShootingHandler>();
        healingHandler = GetComponent<HealingHandler>();

        inventory = GetComponent<Inventory>();

        HandleEvents();
    }

    private void HandleEvents()
    {
        // -------------------------------------------------------------------------------------- BUILDING

        onWallKeyPressed.AddListener(() =>
        {
            buildingHandler.SelectedStructure = StructureType.Wall;
            onBuildingMode.Invoke();
        });

        onFloorKeyPressed.AddListener(() =>
        {
            buildingHandler.SelectedStructure = StructureType.Floor;
            onBuildingMode.Invoke();
        });
        onRampKeyPressed.AddListener(() =>
        {
            buildingHandler.SelectedStructure = StructureType.Ramp;
            onBuildingMode.Invoke();
        });
        onReversedRampKeyPressed.AddListener(() =>
        {
            buildingHandler.SelectedStructure = StructureType.ReversedRamp;
            onBuildingMode.Invoke();
        });

        // -------------------------------------------------------------------------------------- ITEM SLOTS

        onSlot1KeyPressed.AddListener(() =>
        {
            if(inventory.GetActiveSlotIndex() == 0)
            {
                //inventory.CycleBetweenItems(0);
            }
            else
            {
                inventory.SetActiveSlot(0);
            }

            onBuildingMode.Invoke();
        });
        onSlot2KeyPressed.AddListener(() =>
        {
            if (inventory.GetActiveSlotIndex() == 1)
            {
                inventory.CycleBetweenItems(1);
            }
            else
            {
                inventory.SetActiveSlot(1);
            }
            onShootingMode.Invoke();
        });
        onSlot3KeyPressed.AddListener(() =>
        {
            if (inventory.GetActiveSlotIndex() == 2)
            {
                inventory.CycleBetweenItems(2);
            }
            else
            {
                inventory.SetActiveSlot(2);
            }
            onHealingMode.Invoke();
        });

        // -------------------------------------------------------------------------------------- SLOT SWITCHING

        onBuildingMode.AddListener(() =>
        {
            CurrentState = PlayerState.Building;
        });
        onShootingMode.AddListener(() =>
        {
            GameObject activeSlot = inventory.GetActiveSlot();
            inventory.DeactivateAllSlots();
            activeSlot.SetActive(true);
            shootingHandler.ActiveSlot = activeSlot;
            NetworkLog.LogInfoServer($"4 Activated the slot -> {activeSlot}" + $" {NetworkObjectId}");
            shootingHandler.ConfigureWeapon();
            CurrentState = PlayerState.Shooting;
        });
        onHealingMode.AddListener(() =>
        {
            GameObject activeSlot = inventory.GetActiveSlot();
            inventory.DeactivateAllSlots();
            activeSlot.SetActive(true);
            NetworkLog.LogInfoServer($"4 Activated the slot -> {activeSlot}" + $" {NetworkObjectId}");
            healingHandler.ActiveSlot = activeSlot;
            healingHandler.ConfigureHealing();
            CurrentState = PlayerState.Healing;

        });
    }

    private void SwitchStates(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Shooting:
                onShootingMode.Invoke();
                break;
            case PlayerState.Building:
                onBuildingMode.Invoke();
                break;
            case PlayerState.Healing:
                onHealingMode.Invoke();
                break;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        // Handle mode switching
        if (Input.GetKeyDown(KeyCode.Q))
        {
            NotifyWallKeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            NotifyFloorKeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            NotifyRampKeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            NotifyReversedRampKeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            NotifySlot1KeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            NotifySlot2KeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            NotifySlot3KeyPressedServerRpc();
        }

        switch (CurrentState)
        {
            case PlayerState.Building:
                buildingHandler.HandleInput();
                break;
            case PlayerState.Shooting:
                shootingHandler.HandleInput();
                break;
            case PlayerState.Healing:
                healingHandler.HandleInput();
                break;
            default:
                break;
        }
    }

    public HealthSystem GetHealthSystem()
    {
        return GetComponent<HealthSystemComponent>().GetHealthSystem();
    }

    #region RPCs For Key Events

    // Buildings

    [ServerRpc]
    public void NotifyWallKeyPressedServerRpc()
    {
        NotifyWallKeyPressedClientRpc();
    }

    [ClientRpc]
    public void NotifyWallKeyPressedClientRpc()
    {

        onWallKeyPressed.Invoke();
    }

    [ServerRpc]
    public void NotifyFloorKeyPressedServerRpc()
    {
        NotifyFloorKeyPressedClientRpc();
    }

    [ClientRpc]
    public void NotifyFloorKeyPressedClientRpc()
    {
        onFloorKeyPressed.Invoke();
    }

    [ServerRpc]
    public void NotifyRampKeyPressedServerRpc()
    {
        NotifyRampKeyPressedClientRpc();
    }

    [ClientRpc]
    public void NotifyRampKeyPressedClientRpc()
    {
        onRampKeyPressed.Invoke();
    }

    [ServerRpc]
    public void NotifyReversedRampKeyPressedServerRpc()
    {
        NotifyReversedRampKeyPressedClientRpc();
    }

    [ClientRpc]
    public void NotifyReversedRampKeyPressedClientRpc()
    {
        onReversedRampKeyPressed.Invoke();
    }

    // Slots...

    [ServerRpc]
    public void NotifySlot1KeyPressedServerRpc()
    {
        NotifySlot1KeyPressedClientRpc();
    }

    [ClientRpc]
    public void NotifySlot1KeyPressedClientRpc()
    {
        onSlot1KeyPressed.Invoke();
    }
    
    [ServerRpc]
    public void NotifySlot2KeyPressedServerRpc()
    {
        NotifySlot2KeyPressedClientRpc();
    }

    [ClientRpc]
    public void NotifySlot2KeyPressedClientRpc()
    {
        onSlot2KeyPressed.Invoke();
    }
    
    [ServerRpc]
    public void NotifySlot3KeyPressedServerRpc()
    {
        NotifySlot3KeyPressedClientRpc();
    }

    [ClientRpc]
    public void NotifySlot3KeyPressedClientRpc()
    {
        onSlot3KeyPressed.Invoke();
    }
    #endregion  
}