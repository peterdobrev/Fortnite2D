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
    public UnityEvent onSlot4KeyPressed;
    public UnityEvent onSlot5KeyPressed;

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
            inventory.SetActiveSlot(0);
            PlayerState playerState = inventory.DeterminePlayerStateFromItemType(0);
            SwitchStates(playerState);
        });
        onSlot2KeyPressed.AddListener(() =>
        {
            inventory.SetActiveSlot(1);
            PlayerState playerState = inventory.DeterminePlayerStateFromItemType(1);
            SwitchStates(playerState);
        });
        onSlot3KeyPressed.AddListener(() =>
        {
            inventory.SetActiveSlot(2);

            PlayerState playerState = inventory.DeterminePlayerStateFromItemType(2);
            SwitchStates(playerState);
        });
        onSlot4KeyPressed.AddListener(() =>
        {
            inventory.SetActiveSlot(3);
            PlayerState playerState = inventory.DeterminePlayerStateFromItemType(3);
            SwitchStates(playerState);
        });
        onSlot5KeyPressed.AddListener(() =>
        {
            inventory.SetActiveSlot(4);
            PlayerState playerState = inventory.DeterminePlayerStateFromItemType(4);
            SwitchStates(playerState);
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
            shootingHandler.ConfigureWeapon();
            CurrentState = PlayerState.Shooting;
        });
        onHealingMode.AddListener(() =>
        {
            GameObject activeSlot = inventory.GetActiveSlot();
            inventory.DeactivateAllSlots();
            activeSlot.SetActive(true);
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
            onWallKeyPressed.Invoke();
            NotifyWallKeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onFloorKeyPressed.Invoke();
            NotifyFloorKeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            onRampKeyPressed.Invoke();
            NotifyRampKeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            onReversedRampKeyPressed.Invoke();
            NotifyReversedRampKeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            onSlot1KeyPressed.Invoke();
            NotifySlot1KeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            onSlot2KeyPressed.Invoke();
            NotifySlot2KeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            onSlot3KeyPressed.Invoke();
            NotifySlot3KeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            onSlot4KeyPressed.Invoke();
            NotifySlot4KeyPressedServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            onSlot5KeyPressed.Invoke();
            NotifySlot5KeyPressedServerRpc();
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
        if(IsOwner) { return; }
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
        if (IsOwner) { return; }
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
        if (IsOwner) { return; }
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
        if (IsOwner) { return; }
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
        if (IsOwner) { return; }
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
        if (IsOwner) { return; }
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
        if (IsOwner) { return; }
        onSlot3KeyPressed.Invoke();
    }
    
    [ServerRpc]
    public void NotifySlot4KeyPressedServerRpc()
    {
        NotifySlot4KeyPressedClientRpc();
    }

    [ClientRpc]
    public void NotifySlot4KeyPressedClientRpc()
    {
        if (IsOwner) { return; }
        onSlot4KeyPressed.Invoke();
    }
    
    [ServerRpc]
    public void NotifySlot5KeyPressedServerRpc()
    {
        NotifySlot5KeyPressedClientRpc();
    }

    [ClientRpc]
    public void NotifySlot5KeyPressedClientRpc()
    {
        if (IsOwner) { return; }
        onSlot5KeyPressed.Invoke();
    }

#endregion
}