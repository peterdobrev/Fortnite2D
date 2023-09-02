using CodeMonkey.HealthSystemCM;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[RequireComponent(typeof(BuildingHandler))]
[RequireComponent(typeof(ShootingHandler))]
public class PlayerActionController2D : NetworkBehaviour, IGetHealthSystem
{
    public PlayerState CurrentState { get; private set; }
    private BuildingHandler buildingHandler;
    private ShootingHandler shootingHandler;
    private HealingHandler healingHandler;
    private Inventory inventory;

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
            PlayerState playerState = inventory.DeterminePlayerStateFromItemType(0);
            SwitchStates(playerState);
        });
        onSlot2KeyPressed.AddListener(() =>
        {
            PlayerState playerState = inventory.DeterminePlayerStateFromItemType(1);
            SwitchStates(playerState);
        });
        onSlot3KeyPressed.AddListener(() =>
        {
            PlayerState playerState = inventory.DeterminePlayerStateFromItemType(2);
            SwitchStates(playerState);
        });
        onSlot4KeyPressed.AddListener(() =>
        {
            PlayerState playerState = inventory.DeterminePlayerStateFromItemType(3);
            SwitchStates(playerState);
        });
        onSlot5KeyPressed.AddListener(() =>
        {
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
            shootingHandler.activeSlot = activeSlot;
            shootingHandler.ConfigureWeapon();
            CurrentState = PlayerState.Shooting;
        });
        onHealingMode.AddListener(() =>
        {
            GameObject activeSlot = inventory.GetActiveSlot();
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
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onFloorKeyPressed.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            onRampKeyPressed.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            onReversedRampKeyPressed.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            onSlot1KeyPressed.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            onSlot2KeyPressed.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            onSlot3KeyPressed.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            onSlot4KeyPressed.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            onSlot5KeyPressed.Invoke();
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
}