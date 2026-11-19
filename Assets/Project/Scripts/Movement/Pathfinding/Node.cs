using UnityEngine;

public class Node
{
    // grid info. these values are assigned when the node is created and should not change.
    public bool IsWalkable { get; private set; }
    public Vector3 WorldPosition { get; private set; }

    public int GridX { get; private set; }
    public int GridY { get; private set; }

    // A* pathfinding info. will update these values when calculating path.
    // g cost is how far from start node and h cost is how far from destination node.
    // f cost is the sum of the two.
    public int GCost { get; set; }
    public int HCost { get; set; }

    public int FCost => GCost + HCost;

    // this will store the previous node in the path.
    public Node Parent { get; set; }

    public Node(bool isWalkable, Vector3 worldPosition, int gridX, int gridY)
    {
        IsWalkable = isWalkable;
        WorldPosition = worldPosition;
        GridX = gridX;
        GridY = gridY;
    }
}
