using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [Header("Pathfinding Grid")]
    [SerializeField] private PathfindingGrid grid;

    private readonly List<Node> currentPath = new List<Node>();

    // nodes that the algorithm has discovered but still needs to explore.
    private NodeHeap openList;
    // nodes that the algorithm has explored.
    private HashSet<Node> closedList = new();


    public List<Node> FindPath(Vector2 startPosition, Vector2 targetPosition)
    {
        // using A* pathfinding algorithm.

        if (grid == null || grid.Grid == null) return EmptyPath();

        Node startNode = grid.NodeFromWorldPosition(startPosition);
        Node targetNode = grid.NodeFromWorldPosition(targetPosition);

        if (!startNode.IsWalkable || !targetNode.IsWalkable) return EmptyPath();

        InitialiseSearch(startNode, targetNode);

        // go through the open list until there are no nodes left.
        while (openList.Count > 0)
        {
            // get node with lowest cost and mark it as explored.
            Node currentNode = openList.RemoveFirst();
            closedList.Add(currentNode);

            // if we have reached the target retrace the path.
            if (currentNode == targetNode) return RetracePath(startNode, targetNode);

            ProcessNeighbours(currentNode, targetNode);
        }

        // incase the open list is empty and no path can be found.
        return EmptyPath();
    }

    private void InitialiseSearch(Node startNode, Node targetNode)
    {
        // reset lists and nodes.

        // max heap size = grid get row count * grid get column count.
        int maxHeapSize = grid.Grid.GetLength(0) * grid.Grid.GetLength(1);

        // only creating new heap if it doesnt exist or its not big enough,
        // otherwise we can just clear existing one.
        if (openList == null || openList.Capacity != maxHeapSize)
        {
            openList = new NodeHeap(maxHeapSize);
        }
        else
        {
            openList.Clear();
        }

        closedList.Clear();

        foreach (Node node in grid.Grid)
        {
            node.GCost = int.MaxValue;
            node.HCost = 0;
            node.Parent = null;
            node.HeapIndex = -1;
        }

        // initialise and add the starting node to the open list.
        startNode.GCost = 0;
        startNode.HCost = GetDistance(startNode, targetNode);

        openList.Add(startNode);
    }

    private void ProcessNeighbours(Node currentNode, Node targetNode)
    {
        List<Node> neighbours = new(8);

        // getNeighbours fills our neighbours list that we pass in.
        grid.GetNeighbours(currentNode, neighbours);

        // examine all the neighbours.
        foreach (Node neighbour in neighbours)
        {
            // ignoring unwalkable nodes and ones that we have already explored.
            if (!neighbour.IsWalkable || closedList.Contains(neighbour)) continue;

            // ignoring diagonal movement if it would go through a blocked corner.
            if (!CanMoveDiagonally(currentNode, neighbour)) continue;

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
                else
                {
                    openList.UpdateItem(neighbour);
                }
            }
        }
    }

    private bool CanMoveDiagonally(Node currentNode, Node neighbour)
    {
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

        // calculate direction moving in.
        // we can get -1, 0, 1
        // (-1, -1) would be moving down right for example.
        int deltaX = neighbour.GridX - currentNode.GridX;
        int deltaY = neighbour.GridY - currentNode.GridY;

        // if either is zero we are moving horizontally or vertically.
        // so we dont have to check for diagonal movement.
        if (deltaX == 0 || deltaY == 0) return true;

        // get the horizontal and vertical neighbours between the two nodes.
        Node horizontalNeighbour = grid.Grid[currentNode.GridX + deltaX, currentNode.GridY];
        Node verticalNeighbour = grid.Grid[currentNode.GridX, currentNode.GridY + deltaY];

        // diagonal movement is only allowed if both horizontal and vertical neighbour nodes are walkable.
        return horizontalNeighbour.IsWalkable && verticalNeighbour.IsWalkable;
    }

    private List<Node> EmptyPath()
    {
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
}
