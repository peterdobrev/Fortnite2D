using UnityEngine;

public static class StructureOffsetUtility
{
    public static float cellSize = 1f; // will be modified by the grid class on instantiating

    public static Vector2 CalculateOffset(StructureType structure)
    {
        Vector2 offset;
        switch (structure)
        {
            default:
            case StructureType.Wall:
                offset = new Vector2(0, 0.5f);
                break;
            case StructureType.Floor:
                offset = new Vector2(0.5f, 0);
                break;
            case StructureType.Ramp:
            case StructureType.ReversedRamp:
                offset = new Vector2(0.5f, 0.5f);
                break;
        }
        return offset * cellSize;
    }

    public static Quaternion GetRotationForStructure(StructureType structure)
    {
        if (structure == StructureType.Wall || structure == StructureType.Floor)
        {
            return Quaternion.identity;
        }
        else if(structure == StructureType.Ramp) 
        { 
            return Quaternion.Euler(0, 0, 45);
        }
        else
        {
            return Quaternion.Euler(0, 0, -45);
        }
    }
}