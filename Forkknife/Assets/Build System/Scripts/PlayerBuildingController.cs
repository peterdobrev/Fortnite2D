using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ObjectChangeEventStream;

public class PlayerBuildingController : MonoBehaviour
{
    public StructureController structureController;

    private StructureType selectedStructure = StructureType.Wall;

    private void Update()
    {
        SelectStructureBasedOnInput();
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3? buildPosition = GetBuildPosition(mousePosition);
            if (buildPosition != null)  // Vector3.zero is used as an "invalid" flag
            {
                {
                    structureController.Build(selectedStructure, (Vector3)buildPosition);
                }
            }
            // Draw a debug ray to the current calculated build position
            //Debug.DrawRay(transform.position, ((Vector3)buildPosition - transform.position), Color.red);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3? buildPosition = GetBuildPosition(mousePosition);
            if (buildPosition != null)  // Vector3.zero is used as an "invalid" flag
            {
                {
                    structureController.Destroy(selectedStructure, (Vector3)buildPosition);
                }
            }
        }
    }

    private void SelectStructureBasedOnInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedStructure = StructureType.Wall;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            selectedStructure = StructureType.Floor;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            selectedStructure = StructureType.Ramp;
        }
    }

    private Vector3? GetBuildPosition(Vector3 mousePosition)
    {
        var playerCell = GetPlayerCell();
        var mouseCell = GetMouseCell(mousePosition);

        // If the player and the mouse cursor are in the same cell and the selected structure is a Ramp or a Floor, return the player's world position
        if (playerCell == mouseCell && (selectedStructure == StructureType.Ramp || selectedStructure == StructureType.Floor))
        {
            return GetWorldPosition(playerCell);
        }

        Vector3 direction = (mousePosition - transform.position).normalized;

        // Calculate build cell coordinates based on the direction
        Vector2Int buildCell = playerCell + RoundToInt(direction);

       
        buildCell = ApplyStructureSpecificRules(buildCell, playerCell, direction);

        if (structureController.DoesStructureExistAtPosition(AddPositionOffsetBasedOnStructure(selectedStructure, (Vector2)buildCell)))
        {
            Debug.Log("Is occupied");
            return null;
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
        switch (selectedStructure)
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
            case StructureType.Ramp:
                worldPosition += new Vector3(0.5f, 0.5f, 0);
                break;

        }
        return worldPosition;
    }

}
