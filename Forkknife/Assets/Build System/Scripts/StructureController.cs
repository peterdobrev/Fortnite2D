using System.Collections.Generic;
using System;
using UnityEngine;

public class StructureEventArgs : EventArgs
{
    public Vector2 Position { get; set; }
    public StructureType Type { get; set; }

    public StructureEventArgs(Vector2 position)
    {
        Position = position;
    }

    public StructureEventArgs(StructureType type)
    {
        Type = type;
    }

    public StructureEventArgs(Vector2 position, StructureType type)
    {
        Position = position;
        Type = type;
    }
}

public class StructureController : MonoBehaviour
{
    // Build system
    [Tooltip("Build System:")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 originPosition;


    // Prefabs for each type of structure
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject rampPrefab;
    [SerializeField] private GameObject rampReversedPrefab;

    public BuildSystem buildSystem { get; private set; }

    // Mapping from structure positions to their instantiated GameObjects
    private Dictionary<Vector2, GameObject> structureGameObjects;

    private void Start()
    {
        structureGameObjects = new Dictionary<Vector2, GameObject>();

        // Initialize build system
        buildSystem = new BuildSystem(width, height, cellSize, originPosition);

        // Subscribe to build system events
        buildSystem.OnStructureBuilt += HandleStructureBuilt;
        buildSystem.OnStructureDestroyed += HandleStructureDestroyed;   
    }

    public void Build(StructureType structure, Vector3 worldPosition)
    {
        Vector2 offset = CalcPositionOffsetBasedOnStructure(structure);
        Vector2 pos = (Vector2)worldPosition + offset * cellSize;

        if (structureGameObjects.ContainsKey(pos))
        {
            return;
        }

        buildSystem.Build(structure, worldPosition);
    }

    public void Destroy(StructureType structure, Vector3 worldPosition)
    {
        buildSystem.Destroy(structure, worldPosition);
    }

    private Vector2 CalcPositionOffsetBasedOnStructure(StructureType structure)
    {
        Vector2 offset;
        switch(structure)
        {
            default:
            case StructureType.Wall:
                offset = new Vector3(0, 0.5f, 0);
                break;
            case StructureType.Floor:
                offset = new Vector3(0.5f, 0, 0);
                break;
            case StructureType.Ramp:
                offset = new Vector3(0.5f, 0.5f, 0);
                break;
            case StructureType.ReversedRamp:
                offset = new Vector3(0.5f, 0.5f, 0);
                break;

        }
        return offset;
    }

    // Instantiate a structure prefab based on the type of the structure built
    private void HandleStructureBuilt(StructureType structure, StructureEventArgs e)
    {
        GameObject structurePrefab = GetStructurePrefab(e.Type);

        // Ramp should be rotated 45 degrees
        Quaternion quaternion = Quaternion.identity;
        if(e.Type == StructureType.Ramp || e.Type == StructureType.ReversedRamp)
        {
            quaternion = structurePrefab.transform.rotation;
        }

        Vector2 cellWorldPosition = buildSystem.grid.GetWorldPosition((int)e.Position.x, (int)e.Position.y);
        Vector2 offset = CalcPositionOffsetBasedOnStructure(e.Type);

        // world pos
        var instantiatePosition = cellWorldPosition + offset * buildSystem.grid.CellSize;
        // key for dict
        var gameObjectsListPos = e.Position + offset;
        GameObject structureGameObject = Instantiate(structurePrefab, instantiatePosition, quaternion);

        Building buildingComponent = structureGameObject.GetComponent<Building>();
        buildingComponent.OnBuildingDestroyed += HandleBuildingDestruction;

        // Store the game object for future reference
        structureGameObjects[gameObjectsListPos] = structureGameObject;

    }

    // Destroy the structure GameObject when a structure is destroyed in the build system
    private void HandleStructureDestroyed(StructureType structure, StructureEventArgs e)
    {   
        Vector2 offset = CalcPositionOffsetBasedOnStructure(e.Type);
        Vector3 pos = e.Position + offset;

        if(structureGameObjects.TryGetValue(pos, out GameObject structureGameObject))
{
            if (structureGameObject != null)
            {
                Destroy(structureGameObject);
            }
            else
            {
                Debug.LogWarning($"StructureNotFound in Scene but Removed from Dictionary {pos}");
            }
            structureGameObjects.Remove(pos);
        }
    }

    private void HandleBuildingDestruction(Building building)
    {
        Vector2 position = new Vector2(building.transform.position.x, building.transform.position.y);
        StructureType structureType = building.GetComponent<Building>().StructureType;
        Destroy(structureType, position);
    }
    // Return the appropriate prefab based on the type of the structure
    private GameObject GetStructurePrefab(StructureType type)
    {
        switch (type)
        {
            case StructureType.Wall:
                return wallPrefab;
            case StructureType.Floor:
                return floorPrefab;
            case StructureType.Ramp:
                return rampPrefab;
            case StructureType.ReversedRamp:
                return rampReversedPrefab;
            default:
                throw new ArgumentException("Invalid structure type");
        }
    }

    public bool DoesStructureExistAtPosition(Vector2 position)
    {
        // Check if there is a GameObject at this cell position in the structureGameObjects dictionary
        return structureGameObjects.ContainsKey(position);
    }


}
