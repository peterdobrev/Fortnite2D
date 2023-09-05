using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HealingHandler))]
public class BuildingHandler : NetworkBehaviour, IActionHandler
{
    private float lastBuildTime;
    private const float BUILD_COOLDOWN = 0.1f;
    private const string OBSTACLE_LAYER = "Ground"; // Set this to the name of your obstacle layer

    private StructureController structureController;
    private BlueprintHandler blueprintHandler;

    public StructureType SelectedStructure { get; set; }

    public UnityEvent onBuild;


    private void Awake()
    {
        blueprintHandler = GetComponent<BlueprintHandler>();
        structureController = GameObject.FindGameObjectWithTag("BuildSystem").GetComponent<StructureController>();
    }

    private bool IsPathClear(Vector3 start, Vector3 end)
    {
        // Setup the layer mask to only hit obstacles
        int layerMask = 1 << LayerMask.NameToLayer(OBSTACLE_LAYER);

        // Perform the Raycast
        RaycastHit2D hit = Physics2D.Raycast(start, end - start, Vector2.Distance(start, end), layerMask);

        // If we hit something in the obstacle layer, then the path is not clear
        return hit.collider == null;
    }

    public void HandleInput()
    {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPositionCinemachine();

        Vector3? buildPosition = GetBuildPosition(mousePosition);

        if (buildPosition != null &&
            IsPathClear(transform.position, mousePosition) &&
            IsPathClear(transform.position, (Vector3)buildPosition))
        {
            blueprintHandler.ShowBlueprint(SelectedStructure, (Vector3)buildPosition);
        }
        else
        {
            return; // If path isn't clear, simply return from the function.
        }

        if (Input.GetMouseButtonDown(0) && Time.time - lastBuildTime > BUILD_COOLDOWN)
        {
            lastBuildTime = Time.time;
            if (buildPosition != null)
            {
                Debug.Log(buildPosition);
                BuildServerRpc(SelectedStructure, (Vector3)buildPosition);
                blueprintHandler.DestroyBlueprint(); // Destroy blueprint after building
            }
        }
    }

    [ServerRpc]
    private void BuildServerRpc(StructureType type, Vector3 buildPosition)
    {
        structureController.Build(type, buildPosition);
    }

    private Vector3? GetBuildPosition(Vector3 mousePosition)
    {
        Vector2Int mouseCell = GetMouseCell(mousePosition);
        Vector3 cellCenter = (Vector2)GetWorldPosition(mouseCell) + StructureOffsetUtility.CalculateOffset(StructureType.Ramp);  // This is the center node of the cell
        Vector2Int selectedCell;

        switch (SelectedStructure)
        {
            case StructureType.Ramp:
            case StructureType.ReversedRamp:
                selectedCell = mouseCell;
                break;

            case StructureType.Wall:
                if (mousePosition.x > cellCenter.x)
                {
                    // Go to the next cell to the right and get its left node
                    selectedCell = new Vector2Int(mouseCell.x + 1, mouseCell.y);
                }
                else
                {
                    // Use the left node of the current cell
                    selectedCell = mouseCell;
                }
                break;

            case StructureType.Floor:
                if (mousePosition.y > cellCenter.y)
                {
                    // Go to the cell above and get its bottom node
                    selectedCell = new Vector2Int(mouseCell.x, mouseCell.y + 1);
                }
                else
                {
                    // Use the bottom node of the current cell
                    selectedCell = mouseCell;
                }
                break;

            default:
                return null;
        }
        if(!structureController.buildSystem.grid.IsCellValid(selectedCell.x, selectedCell.y))
        {
            return null;
        }
        var structurePos = (Vector2)GetWorldPosition(selectedCell) + StructureOffsetUtility.CalculateOffset(SelectedStructure);
        var structurePosForList = selectedCell + StructureOffsetUtility.CalculateOffset(SelectedStructure) / StructureOffsetUtility.cellSize;
        // If the structure does not exist at the selected position, return the position. Otherwise, return null.
        if (!structureController.DoesStructureExistAtPosition(structurePosForList))
        {
            return (Vector2)structurePos;
        }
        else
        {
            Debug.Log("Position occupied");
            return null;
        }
    }


    private Vector2Int GetPlayerCell()
    {
        int playerX, playerY;
        structureController.buildSystem.grid.GetXY(transform.position, out playerX, out playerY);
        return new Vector2Int(playerX, playerY);
    }

    private Vector2Int GetMouseCell(Vector3 mousePosition)
    {
        int mouseX, mouseY;
        structureController.buildSystem.grid.GetXY(mousePosition, out mouseX, out mouseY);
        return new Vector2Int(mouseX, mouseY);
    }

    private Vector2Int RoundToInt(Vector3 direction)
    {
        return new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
    }

    private Vector3? GetWorldPosition(Vector2Int cell)
    {
        return structureController.buildSystem.grid.GetWorldPosition(cell.x, cell.y);
    }
}