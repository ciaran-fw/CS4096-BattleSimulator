using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [Header("Pathfinding Grid")]
    [SerializeField] private PathfindingGrid grid;

    [Header("Pathfinding Test")]
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform targetPosition;
    

    private List<Node> currentPath = new List<Node>();

    // nodes that the algorithm has discovered but still needs to explore.
    private List<Node> openList = new();
    // nodes that the algorithm has explored.
    private HashSet<Node> closedList = new();

    private Node GetLowestFCostNode()
    {
        Node lowestCostNode = openList[0];

        for (int i = 1; i < openList.Count; i++)
        {
            Node currentNode = openList[i];

            // if f costs are the same we compare h costs.
            // if both f cost and h cost are the same we just go with node,
            // we already have.
            if (
                currentNode.FCost < lowestCostNode.FCost ||
                (currentNode.FCost == lowestCostNode.FCost) && (currentNode.HCost < lowestCostNode.HCost)
            )
            {
                lowestCostNode = currentNode;
            }
        }

        return lowestCostNode;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        // we use octile distance as our representation of distance.

        // diagonal movement costs 14 and straight movement costs 10.
        // we do this instead of using 1 and 1.414 to avoid floating point,
        // precision errors and also because its just simpler.

        // working out example to try and understand it.
        // eg node a = (2, 3), node b = (5, 5)
        // distanceX = Abs(2 - 5) = Abs(-3) = 3
        // distanceY = 2
        // diagonalDistance = Min(3, 2) = 2
        // straightDistance = Abs(3 - 2) = Abs(1) = 1
        // distance = (2 * 14) + (1 * 10) = 28 + 10 = 38

        int distanceX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int distanceY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        int diagonalDistance = Mathf.Min(distanceX, distanceY);
        int straightDistance = Mathf.Abs(distanceX - distanceY);

        return (diagonalDistance * 14) + (straightDistance * 10);
    }

    [ContextMenu("Find Path")]
    public void FindPath()
    {
        if (startPosition == null || targetPosition == null)
        {
            Debug.LogError("Error - start position or target position not set..");
            return;
        }

        currentPath = FindPath(startPosition.position, targetPosition.position);

        Debug.Log($"Path found containing {currentPath.Count} nodes.");
    }

    public List<Node> FindPath(Vector2 startPosition, Vector2 targetPosition)
    {
        // using A* pathfinding algorithm.

        if (grid == null || grid.Grid == null)
        {
            Debug.LogError("Error- pathfinding grid not created.");
            return new List<Node>();
        }

        Node startNode = grid.NodeFromWorldPosition(startPosition);
        Node targetNode = grid.NodeFromWorldPosition(targetPosition);

        // checking that both positions are walkable.
        if (!startNode.IsWalkable || !targetNode.IsWalkable)
        {
            return new List<Node>();
        }

        // reset lists and nodes.
        openList.Clear();
        closedList.Clear();

        foreach (Node node in grid.Grid)
        {
            node.GCost = int.MaxValue;
            node.HCost = 0;
            node.Parent = null;
        }

        List<Node> neighbours = new(8);

        // add the starting node to the open list.
        openList.Add(startNode);

        // we go through the open list until there is no nodes left,
        // basically we are searching the entire grid.
        while (openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode();

            // mark as explored.
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // if we have reached the target retrace the path.
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            // getNeighbours fills our neighbours list that we pass in.
            // trying to improve performance better than creating new list,
            // every time.
            grid.GetNeighbours(currentNode, neighbours);

            // examine all the neighbours.
            foreach (Node neighbour in neighbours)
            {
                // ignoring unwalkable nodes and ones that we have already explored.
                if (!neighbour.IsWalkable || closedList.Contains(neighbour)) continue;

                // prevent diagonal movement through blocked corners.
                // example:
                // (3, 6),  (4, 6),  (5, 6)
                // (3, 5),  (4, 5),  (5, 5)
                // (3, 4),  (4, 4),  (5, 4)
                // if we want to go to from current node (4, 5) to neighbour (3, 6)
                // deltaX = 3 - 4 = -1
                // deltaY = 6 - 5 = 1
                // we are moving up left. lets get neighbours we need to check:
                // horizontal neighbour = Grid[4 + -1, 5] = Node at (3, 5)
                // vertical neighbour = Grid[4, 5 + 1] = Node at (4, 6)
                // we need to check if either are then unwalkable.
                // we dont want unit squeezing past unwalkable nodes if a diagonal path,
                // is possible.
                int deltaX = neighbour.GridX - currentNode.GridX;
                int deltaY = neighbour.GridY - currentNode.GridY;

                if (deltaX != 0 && deltaY != 0)
                {
                    Node horizontalNeighbour = grid.Grid[currentNode.GridX + deltaX, currentNode.GridY];
                    Node verticalNeighour = grid.Grid[currentNode.GridX, currentNode.GridY + deltaY];

                    if (!horizontalNeighbour.IsWalkable || !verticalNeighour.IsWalkable) continue;
                }

                // calculate cost of reaching the neighbour.
                // the total cost to get to current node plus cost to move to neighbour.
                int newMovementCost = currentNode.GCost + GetDistance(currentNode, neighbour);

                // update neighbour if we have found cheaper/better route.
                if (newMovementCost < neighbour.GCost)
                {
                    neighbour.GCost = newMovementCost;
                    neighbour.HCost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = currentNode;

                    // add neighbour to open list if not already there.
                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }

        // incase the open list is empty and no path can be found.
        return new List<Node>();
    }

    private List<Node> RetracePath(Node startNode, Node targetNode)
    {
        List<Node> path = new();

        Node currentNode = targetNode;

        // keep following parent refrences back to starting node.
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        // reverse the path so that it starts from starting node,
        // goinf to target node.
        path.Reverse();

        return path;
    }

    private void OnDrawGizmos()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        Gizmos.color = Color.blue;

        foreach (Node node in currentPath)
        {
            Gizmos.DrawCube(node.WorldPosition, Vector3.one * 0.4f);
        }
    }
}
