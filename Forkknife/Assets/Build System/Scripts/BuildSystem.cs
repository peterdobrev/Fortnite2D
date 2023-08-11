using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BuildSystem
{
    public Grid<Cell> grid { get; private set; }
    private Dictionary<Vector2, List<Node>> adjacencyList = new Dictionary<Vector2, List<Node>>();

    private List<Vector2Int> directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1),    // North
        new Vector2Int(1, 1),    // Northeast
        new Vector2Int(1, 0),    // East
        new Vector2Int(1, -1),   // Southeast
        new Vector2Int(0, -1),   // South
        new Vector2Int(-1, -1),  // Southwest
        new Vector2Int(-1, 0),   // West
        new Vector2Int(-1, 1)    // Northwest
    };

    public event Action<StructureType, StructureEventArgs> OnStructureBuilt;
    public event Action<StructureType, StructureEventArgs> OnStructureDestroyed;


    public BuildSystem(int width, int height, float cellSize, Vector3 originPosition)
    {
        grid = new Grid<Cell>(width, height, cellSize, originPosition, (Grid<Cell> g, int x, int y) => new Cell());
    }

    public void Build(StructureType structure, Vector3 worldPosition)
    {
        Cell cell = grid.GetGridObject(worldPosition);

        if (cell == null)
        {
            return;
        }

        Node node = new Node(structure);  

        if (Mathf.Approximately(GetGridPosition(worldPosition).y, grid.GetOriginPosition().y))
        {
            node.IsGround = true;
        }


        switch (structure)
        {
            case StructureType.Wall:
                cell.LeftEdgeNode = node;  
                break;
            case StructureType.Floor:
                cell.BottomEdgeNode = node;  
                break;
            case StructureType.Ramp:
                cell.CenterNode = node; 
                break;
        }


        Vector2Int gridPos = GetGridPosition(worldPosition);
        AddToAdjacencyList(gridPos, node); 

        foreach (Vector2 direction in directions)
        {
            Vector2 adjacentPosition = gridPos + direction;
            if (adjacencyList.TryGetValue(adjacentPosition, out List<Node> adjacentNodes))
            {
                foreach (Node adjacentNode in adjacentNodes)
                {
                    if (CanConnect(node.Structure, adjacentNode.Structure, direction))
                    {
                        ConnectNodes(node, adjacentNode);
                    }
                }
            }
        }

        //Destroy structure if in the air
        if (!CheckAdjacency(gridPos))
        {
            Destroy(structure, worldPosition);
            return;
        }

        StructureEventArgs structureArgs = new StructureEventArgs(gridPos, structure);
        OnStructureBuilt?.Invoke(structure, structureArgs);
    }

    public void Destroy(StructureType structure, Vector3 worldPosition)
    {
        Cell cell = grid.GetGridObject(worldPosition);

        if (cell == null)
        {
            return;
        }

        Node node;

        switch (structure)
        {
            default:
            case StructureType.Wall:
                node = cell.LeftEdgeNode;
                cell.LeftEdgeNode = null;
                break;
            case StructureType.Floor:
                node = cell.BottomEdgeNode;
                cell.BottomEdgeNode = null;
                break;
            case StructureType.Ramp:
                node = cell.CenterNode;
                cell.CenterNode = null;
                break;
        }

        Vector2Int gridPos = GetGridPosition(worldPosition);
        StructureEventArgs structureArgs = new StructureEventArgs(gridPos, structure);
        OnStructureDestroyed?.Invoke(structure, structureArgs);

        RemoveFromAdjacencyList(gridPos, node);

        foreach (Node adjacentNode in node.AdjacentNodes)
        {
            adjacentNode.AdjacentNodes.Remove(node);
        }
        node.AdjacentNodes.Clear();
        DestroyFloatingStructures();

    }

    public void AddToAdjacencyList(Vector2Int gridPosition, Node node)
    {
        if (!adjacencyList.TryGetValue(gridPosition, out List<Node> nodesAtPosition))
        {
            nodesAtPosition = new List<Node>();
            adjacencyList[gridPosition] = nodesAtPosition;
        }

        nodesAtPosition.Add(node);

        foreach (Node existingNode in nodesAtPosition)
        {
            if (existingNode != node) 
            {
                ConnectNodes(existingNode, node);
            }
        }
    }

    public void RemoveFromAdjacencyList(Vector2Int gridPosition, Node node)
    {
        if (adjacencyList.TryGetValue(gridPosition, out List<Node> nodesAtPosition))
        {
            nodesAtPosition.Remove(node);

            if (nodesAtPosition.Count == 0)
            {
                adjacencyList.Remove(gridPosition);
            }
        }
    }

    public bool CheckAdjacency(Vector2Int gridPosition)
    {
        if (adjacencyList.TryGetValue(gridPosition, out List<Node> nodesAtPosition))
        {
            foreach (Node node in nodesAtPosition)
            {
                if (node.AdjacentNodes.Count > 0)
                {
                    return true;
                }
                else if (node.IsGround)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool CanConnect(StructureType structure1, StructureType structure2, Vector2 direction)
    {
        switch (structure1)
        {
            case StructureType.Wall:
                if (structure2 == StructureType.Wall)
                {
                    return direction == Vector2Int.up || direction == Vector2Int.down;
                }
                else if (structure2 == StructureType.Floor)
                {
                    return direction == Vector2Int.left || direction == new Vector2Int(-1, 1) || direction == Vector2Int.up;
                }
                else if (structure2 == StructureType.Ramp)
                {
                    return direction == Vector2Int.left || direction == Vector2Int.up || direction == new Vector2Int(-1, -1);
                }
                break;

            case StructureType.Floor:
                if (structure2 == StructureType.Floor)
                {
                    return direction == Vector2Int.right || direction == Vector2Int.left;
                }
                else if (structure2 == StructureType.Wall)
                {
                    return direction == Vector2Int.right || direction == new Vector2Int(1, -1) || direction == Vector2Int.down;
                }
                else if (structure2 == StructureType.Ramp)
                {
                    return direction == Vector2Int.right || direction == Vector2Int.down || direction == new Vector2Int(-1, -1);
                }
                break;

            case StructureType.Ramp:
                if (structure2 == StructureType.Ramp)
                {
                    return direction == new Vector2Int(1, 1) || direction == new Vector2Int(-1, -1);
                }
                else if (structure2 == StructureType.Floor)
                {
                    return direction == Vector2Int.up || direction == new Vector2Int(1, 1) || direction == Vector2Int.left;
                }
                else if (structure2 == StructureType.Wall)
                {
                    return direction == Vector2Int.right || direction == new Vector2Int(1, 1) || direction == Vector2Int.down;
                }
                break;
        }
        return false;
    }

    private void ConnectNodes(Node node1, Node node2)
    {
        if (!node1.AdjacentNodes.Contains(node2))
        {
            node1.AdjacentNodes.Add(node2);
        }

        if (!node2.AdjacentNodes.Contains(node1))
        {
            node2.AdjacentNodes.Add(node1);
        }
    }

    private void DestroyFloatingStructures()
    {
        HashSet<Node> visited = new HashSet<Node>();
        List<Node> groundNodes = GetGroundNodes();

        foreach (Node groundNode in groundNodes)
        {
            DFS(groundNode, visited);
        }

        // Nodes -> <Vector2, Node>
        foreach (var nodes in adjacencyList)
        {
            foreach (Node node in nodes.Value)
            {
                if (!visited.Contains(node))
                {
                    Destroy(node.Structure, grid.GetWorldPosition((int)nodes.Key.x, (int)nodes.Key.y)); // nodes.Key.x and .y are the coordinates of the cell that contains the nodes
                }
            }
        }
    }

    private void DFS(Node node, HashSet<Node> visited)
    {
        if (visited.Contains(node)) return;
        visited.Add(node);

        foreach (Node adjacentNode in node.AdjacentNodes)
        {
            DFS(adjacentNode, visited);
        }
    }

    private List<Node> GetGroundNodes()
    {
        List<Node> groundNodes = new List<Node>();

        foreach (KeyValuePair<Vector2, List<Node>> entry in adjacencyList)
        {
            foreach (Node node in entry.Value)
            {
                if (node.IsGround)
                {
                    groundNodes.Add(node);
                }
            }
        }

        return groundNodes;
    }

    // This function converts a world position to a grid position
    private Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXY(worldPosition, out int x, out int y);
        return new Vector2Int(x, y);
    }
}
