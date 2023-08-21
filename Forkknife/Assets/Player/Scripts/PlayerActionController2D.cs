using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BuildingHandler))]
[RequireComponent(typeof(ShootingHandler))]
public class PlayerActionController2D : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; }
    private BuildingHandler buildingHandler;
    private ShootingHandler shootingHandler;

    private PhotonView view;

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

    private void Awake()
    {
        CurrentState = PlayerState.Shooting;

        view = GetComponent<PhotonView>();

        buildingHandler = GetComponent<BuildingHandler>();
        shootingHandler = GetComponent<ShootingHandler>();

        HandleEvents();
    }

    private void HandleEvents()
    {
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
        onSlot1KeyPressed.AddListener(() =>
        {
            onShootingMode.Invoke();
        });

        onBuildingMode.AddListener(() =>
        {
            CurrentState = PlayerState.Building;
        });
        onShootingMode.AddListener(() =>
        {
            CurrentState = PlayerState.Shooting;
        });
    }

    void Update()
    {
        if(view.IsMine)
        {
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

            switch (CurrentState)
            {
                case PlayerState.Building:
                    buildingHandler.HandleInput();
                    break;
                case PlayerState.Shooting:
                    shootingHandler.HandleInput();
                    break;
                default:
                    break;
            }
        }
    }
}