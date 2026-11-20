using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid : MonoBehaviour
{
    [Header("Grid Boundaries")]
    // bottom left corner of the grid. this is in world coordinates.
    [SerializeField] private Vector2 gridStartPosition = new(40f, 40f);
    // top right corner of the grid. this is in world coordinates.
    [SerializeField] private Vector2 gridEndPosition = new(40f, 40f);

    [Header("Grid Settings")]
    [SerializeField] private float nodeRadius = 0.5f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Gizmo Visualization Settings")]
    [Header("Outline")]
    [SerializeField] private bool drawOutlineGizmo = true;
    [SerializeField] private Color outlineColour = Color.white;
    [Header("Nodes")]
    [SerializeField] private bool drawNodesGizmo = true;
    [SerializeField] private float nodeSeperation = 0.1f;
    [Range(0f, 1f)] [SerializeField] private float nodeAlpha = 0.6f;
    [SerializeField] private Color walkableColour = Color.green;
    [SerializeField] private Color unwalkableColour = Color.red;

    // stores all the nodes in the grid.
    public Node[,] Grid { get; private set; }

    // grid size measured in nodes.
    private int gridSizeX;
    private int gridSizeY;

    // actual size of a node - 2 times the radius.
    private float NodeSize => nodeRadius * 2f;

    private Vector2 GridCentrePosition => (gridStartPosition + gridEndPosition) / 2f;
    private Vector2 GridSize => gridEndPosition - gridStartPosition;


    private void Start()
    {
        CreateGrid();
    }

    // ----------------------------- GRID CREATION -----------------------------

    public void CreateGrid()
    {
        // just to make sure values where inputed by me or teammate correctly,
        // we check if nodes size is okay and that end position is not before,
        // the start position.
        if (nodeRadius <= 0f)
        {
            Debug.LogError("Error - node radius must be greater than zero.");
            return;
        }

        if (gridEndPosition.x <= gridStartPosition.x || gridEndPosition.y <= gridStartPosition.y)
        {
            Debug.LogError("Error - invalid grid starting and end positions.");
            return;
        }

        // calculate how many nodes can fit in the grid.
        // we are using floor as we dont want nodes outside of the grid,
        // which could happen if numbers were instead rounded up.
        gridSizeX = Mathf.FloorToInt(GridSize.x / NodeSize);
        gridSizeY = Mathf.FloorToInt(GridSize.y / NodeSize);

        // just incase make sure grid is large enough to fit at least one node.
        if (gridSizeX <= 0 || gridSizeY <= 0)
        {
            Debug.LogError("Error - grid dimensions are too small.");
            return;
        }

        Grid = new Node[gridSizeX, gridSizeY];

        // creating each node.
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // find world position that would be at center of node.
                Vector2 worldPosition = new(
                    gridStartPosition.x + nodeRadius + x * NodeSize,
                    gridStartPosition.y + nodeRadius + y * NodeSize
                );

                // check if an obstacle is over this node.
                // if so node is not walkable.
                bool obstacleOverlap = Physics2D.OverlapBox(
                    worldPosition,
                    Vector2.one * NodeSize,
                    0f,
                    obstacleMask
                );

                bool isWalkable = !obstacleOverlap;

                // create node and add to grid.
                Grid[x, y] = new Node(isWalkable, worldPosition, x, y);
            }
        }

        Debug.Log($"Pathfinding grid created successfully containing ({gridSizeX} x {gridSizeY}) nodes.");
    }

    // ----------------------------- NODE OPERATIONS -----------------------------

    public Node NodeFromWorldPosition(Vector2 worldPosition)
    {
        // calculate how far the position is across the grid 0 - 1.
        float percentX = (worldPosition.x - gridStartPosition.x) / GridSize.x;
        float percentY = (worldPosition.y - gridStartPosition.y) / GridSize.y;

        // clamping positions between 0 and 1 so that positions outside the,
        // grid go to nearest edge.
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // converting percentages into grid array indices.
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return Grid[x, y];
    }

    public void GetNeighbours(Node node, List<Node> neighbours)
    {
        neighbours.Clear();

        // we need to check the eight positions surrounding the node.
        // will give us:
        // (-1, 1),  (0, 1),  (1, 1)
        // (-1, 0),  CURRENT, (1, 0)
        // (-1, -1), (0, -1), (1, -1)
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // skip the node we are getting neighbours of.
                if (x == 0 && y == 0) continue;

                // calculate neighbours grid coordinates.
                int neighbourX = node.GridX + x;
                int neighbourY = node.GridY + y;

                // make sure coordinates are not outside the grid.

                bool validX = neighbourX >= 0 && neighbourX < gridSizeX;
                bool validY = neighbourY >= 0 && neighbourY < gridSizeY;

                if (validX && validY)
                {
                    neighbours.Add(Grid[neighbourX, neighbourY]);
                }
            }
        }
    }

    // ----------------------------- GIZMOS VISUALIZATION -----------------------------

    private void OnDrawGizmos()
    {
        if (!drawOutlineGizmo && !drawNodesGizmo) return;

        // draw wire cube to show grid size.
        if (drawOutlineGizmo)
        {
            Gizmos.color = outlineColour;
            Gizmos.DrawWireCube(GridCentrePosition, GridSize);
        }
        
        if (drawNodesGizmo)
        {
            // we will only draw nodes is grid has been created.
            if (Grid == null) return;

            // make sure that the seperation is valid.
            if (nodeSeperation >= NodeSize || nodeSeperation <= 0f)
            {
                Debug.LogError("Error - seperation cannot be greater than or equal to size.");
                return;
            }

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Node node = Grid[x, y];

                    Color nodeColour = node.IsWalkable ? walkableColour : unwalkableColour;
                    nodeColour.a = nodeAlpha;

                    Gizmos.color = nodeColour;

                    // we will draw a small square to display node.
                    Vector3 cubeSize = new Vector3(
                        NodeSize - nodeSeperation,
                        NodeSize - nodeSeperation,
                        nodeSeperation
                    );

                    Gizmos.DrawCube(node.WorldPosition, cubeSize);
                }
            }
        }
    }
}
