using System.Collections.Generic;

public class Node
{
    public StructureType Structure { get; set; }
    public List<Node> AdjacentNodes { get; set; }
    public bool IsGround { get; set; }

    public Node(StructureType structure)
    {
        Structure = structure;
        AdjacentNodes = new List<Node>();
    }
}
