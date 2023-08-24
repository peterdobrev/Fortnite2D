using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildingHandler : MonoBehaviour, IActionHandler
{
    private float lastBuildTime; 
    private const float BUILD_COOLDOWN = 0.1f;
    public StructureType SelectedStructure { get; set; }

    public BlueprintHandler blueprintHandler;

    public StructureController structureController;

    public UnityEvent onBuild;

    public void HandleInput()
    {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();

        Vector3? buildPosition = GetBuildPosition(mousePosition);
        if (buildPosition != null)
        {
            blueprintHandler.ShowBlueprint(SelectedStructure, (Vector3)buildPosition);
        }

        if (Input.GetMouseButtonDown(0) && Time.time - lastBuildTime > BUILD_COOLDOWN)
        {
            lastBuildTime = Time.time;
            if (buildPosition != null)
            {
                structureController.Build(SelectedStructure, (Vector3)buildPosition);
                blueprintHandler.DestroyBlueprint(); // Destroy blueprint after building
            }
        }
    }

    private Vector3? GetBuildPosition(Vector3 mousePosition)
    {
        var playerCell = GetPlayerCell();
        var mouseCell = GetMouseCell(mousePosition);

        Vector3 direction = (mousePosition - transform.position).normalized;

        // Calculate build cell coordinates based on the direction
        Vector2Int buildCell = playerCell + RoundToInt(direction);
        buildCell = ApplyStructureSpecificRules(buildCell, playerCell, direction);

        // Check if the potential build position is occupied
        if (structureController.DoesStructureExistAtPosition(AddPositionOffsetBasedOnStructure(SelectedStructure, (Vector2)buildCell)))
        {
            Debug.Log("Is occupied");
            return null;
        }

        // If the player and the mouse cursor are in the same cell and the selected structure is a Ramp or a Floor, return the player's world position
        if (playerCell == mouseCell && (SelectedStructure == StructureType.Ramp || SelectedStructure == StructureType.Floor || SelectedStructure == StructureType.ReversedRamp))
        {
            if (!structureController.DoesStructureExistAtPosition(AddPositionOffsetBasedOnStructure(SelectedStructure, (Vector2)playerCell)))
                return GetWorldPosition(playerCell);
        }

        if (structureController.buildSystem.grid.IsCellValid(buildCell.x, buildCell.y))
        {
            // If the cell is valid, convert it back to world position
            return GetWorldPosition(buildCell);
        }

        // If the cell is not valid, return null
        return null;
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

    private Vector2Int ApplyStructureSpecificRules(Vector2Int buildCell, Vector2Int playerCell, Vector3 direction)
    {
        switch (SelectedStructure)
        {
            case StructureType.Wall:
                if (buildCell.x < playerCell.x) // Wall is being placed to the left of the player
                {
                    buildCell.x += 1; // Place the wall one cell to the left
                }
                break;

            case StructureType.Ramp:
            case StructureType.Floor:
                if (playerCell == buildCell)
                {
                    return buildCell;
                }
                else if (buildCell.y < playerCell.y) // Floor is being placed below the player
                {
                    buildCell.y += 1; // Place the floor one cell above
                }
                break;
        }

        return buildCell;
    }

    private Vector3 AddPositionOffsetBasedOnStructure(StructureType structure, Vector3 worldPosition)
    {
        switch (structure)
        {
            default:
            case StructureType.Wall:
                worldPosition += new Vector3(0, 0.5f, 0);
                break;
            case StructureType.Floor:
                worldPosition += new Vector3(0.5f, 0, 0);
                break;
            case StructureType.Ramp or StructureType.ReversedRamp:
                worldPosition += new Vector3(0.5f, 0.5f, 0);
                break;

        }
        return worldPosition;
    }
}