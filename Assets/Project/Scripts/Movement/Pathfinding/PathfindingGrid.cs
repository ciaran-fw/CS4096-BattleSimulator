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
    [SerializeField] private bool drawGridOutline = true;
    [SerializeField] private Color outlineColour = Color.white;
    [Header("Nodes")]
    [SerializeField] private float nodeSeperation = 0.1f;
    [Range(0f, 1f)]
    [SerializeField] private float nodeAlpha = 0.6f;
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

        Debug.Log($"Pathfinding grid created successfully ({gridSizeX} x {gridSizeY}) containing nodes.");
    }

    private void OnDrawGizmos()
    {
        // draw wire cube to show grid size.
        if (drawGridOutline)
        {
            Gizmos.color = outlineColour;
            Gizmos.DrawWireCube(GridCentrePosition, GridSize);
        }

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

                Color nodeColour = node.IsWalkable ? walkableColour: unwalkableColour;
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
