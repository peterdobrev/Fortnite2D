using System;
using UnityEngine;

public class BlueprintHandler : MonoBehaviour
{
    [SerializeField] private GameObject wallBlueprintPrefab;
    [SerializeField] private GameObject floorBlueprintPrefab;
    [SerializeField] private GameObject rampBlueprintPrefab;
    [SerializeField] private GameObject rampReversedBlueprintPrefab;

    private GameObject currentBlueprint;

    public void ShowBlueprint(StructureType structure, Vector3 position)
    {
        if (currentBlueprint != null)
        {
            Destroy(currentBlueprint);
        }

        Vector2 offset = StructureOffsetUtility.CalculateOffset(structure);
        position += new Vector3(offset.x, offset.y, 0);

        GameObject blueprintPrefab = GetBlueprintPrefab(structure);
        currentBlueprint = Instantiate(blueprintPrefab, position, StructureOffsetUtility.GetRotationForStructure((structure)));
    }


    private GameObject GetBlueprintPrefab(StructureType type)
    {
        switch (type)
        {
            case StructureType.Wall:
                return wallBlueprintPrefab;
            case StructureType.Floor:
                return floorBlueprintPrefab;
            case StructureType.Ramp:
                return rampBlueprintPrefab;
            case StructureType.ReversedRamp:
                return rampReversedBlueprintPrefab;
            default:
                throw new ArgumentException("Invalid structure type");
        }
    }

    public void DestroyBlueprint()
    {
        if (currentBlueprint != null)
        {
            Destroy(currentBlueprint);
        }
    }
}
